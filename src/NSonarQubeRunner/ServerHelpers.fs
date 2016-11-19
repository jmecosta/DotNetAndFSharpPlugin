﻿module ServerHelpers

open System
open System.Collections.Immutable
open System.Collections.Generic
open System.IO
open System.Reflection

open Microsoft.CodeAnalysis

open VSSonarPlugins
open VSSonarPlugins.Types

open Types

let GetSonarLintDiagnostics(externlProfileIn : System.Collections.Generic.Dictionary<string, Profile>, notificationManager : INotificationManager, checksRoslyn : System.Collections.Generic.List<DiagnosticAnalyzerType>) =
    let mutable checksAvailalbe = ""
    let mutable builder = List.Empty
    let mutable ids = List.Empty
    for check in checksRoslyn do

            let mutable checkadded = false
            for diagnostic in check.Diagnostic.SupportedDiagnostics do
                if not(checkadded) then
                    let id = "csharpsquid:" + diagnostic.Id
                    checksAvailalbe <- checksAvailalbe + " - " + id
                    try
                        let rule = externlProfileIn.["cs"].GetRule(id)
                        if rule <> null then
                            builder <- builder @ [check]
                            ids <- ids @ [new KeyValuePair<string, ReportDiagnostic>(diagnostic.Id, ReportDiagnostic.Warn)]

                            if rule.Params.Count <> 0 then
                                checkadded <- true
                                let fields = check.Diagnostic.GetType().GetProperties()
                                for field in fields do
                                    let props = field.CustomAttributes
                                    let attributes = field.GetCustomAttributes().ToImmutableArray()
                                    if attributes.Length = 1 &&
                                        attributes.[0].TypeId.ToString().EndsWith("Common.RuleParameterAttribute") then
                                        try
                                            let typeOfField = field.PropertyType
                                            let typeOfFiledName = field.PropertyType.Name
                                            if typeOfFiledName.Equals("IImmutableSet`1") then
                                                let elems = rule.Params.[0].DefaultValue.Replace("\"", "").Split(',').ToImmutableHashSet()
                                                field.SetValue(check.Diagnostic, elems)
                                            else
                                                let changedValue = Convert.ChangeType(rule.Params.[0].DefaultValue.Replace("\"", ""), typeOfField)
                                                field.SetValue(check.Diagnostic, changedValue)

                                            let value = field.GetValue(check.Diagnostic)
                                            notificationManager.ReportMessage(new Message(Id = "NSonarQubeRunner", Data = "Applied Rule Parameter csharpsquid:" + diagnostic.Id + " = " + rule.Params.[0].DefaultValue))
                                            System.Diagnostics.Debug.WriteLine("Applied Rule Parameter csharpsquid:" + diagnostic.Id + " = " + rule.Params.[0].DefaultValue)
                                        with
                                        | ex -> 
                                            notificationManager.ReportMessage(new Message(Id = "NSonarQubeRunner", Data = "Rule parameter was not applied csharpsquid:" + diagnostic.Id + " = " + rule.Params.[0].DefaultValue))
                                            System.Diagnostics.Debug.WriteLine("Failed to apply Rule Parameter csharpsquid:" + diagnostic.Id + " = " + rule.Params.[0].DefaultValue)
                                ()
                        with
                        | ex -> System.Diagnostics.Debug.WriteLine("Cannot Add Check Failed: " + check.ToString() + " : " +  ex.Message)
                                notificationManager.ReportMessage(new Message(Id = "NSonarQubeRunner", Data = "Check Rule Could Not Be Used " + id + " : " + ex.Message))

    builder, ids

let GetRoslynPluginProps(configuration : ISonarConfiguration,
                            resource : Resource,
                            service : ISonarRestService,
                            notificationManager : INotificationManager) =

    let mutable synctype = true
    let mutable diagnostics = list.Empty
    let mutable additionalDocs = list.Empty

    let properties = service.GetProperties(configuration, resource)
    let mutable additionalFiles : Map<string, string> = Map.empty
            
    for prop in properties do
        if prop.Key.Equals("sonar.roslyn.sync.type") then
            synctype <- prop.Value.ToLower().Equals("true")

        if prop.Key.StartsWith("sonar.roslyn.additional.files.") &&  prop.Key.EndsWith(".sonar.roslyn.additional.name") then
            let name = prop.Value
            let epoch = prop.Key.Replace(".sonar.roslyn.additional.name", "")
            let valuematch = properties |> Seq.tryFind (fun c -> c.Key.StartsWith(epoch) && not(c.Key.EndsWith(".sonar.roslyn.additional.name")))
            let content = 
                match valuematch with
                | Some c -> c.Value
                | _ -> ""

            if content <> "" && not(additionalFiles.ContainsKey(name))  then
                additionalFiles <- additionalFiles.Add(name, content)

            ()

    let gettmppath = Path.Combine(Path.GetTempPath(), "diagnostics")

    if Directory.Exists(gettmppath) then 
        Directory.Delete(gettmppath, true)

    Directory.CreateDirectory(gettmppath) |> ignore

    for file in additionalFiles do
            
        let filename = Path.Combine(gettmppath, file.Key)
        try
            File.WriteAllText(filename, file.Value)
            additionalDocs <- additionalDocs @ [filename]
        with
        | ex -> notificationManager.ReportMessage(new Message (Id = "RoslynRunner", Data = "Failed to create additional file : " + ex.Message))

    synctype, additionalDocs, true
