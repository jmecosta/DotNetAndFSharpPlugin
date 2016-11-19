namespace FSharpLintRunner

open System
open System.Resources
open System.Reflection
open System.Globalization
open System.Collections
open System.IO


open VSSonarPlugins.Types
open VSSonarPlugins

open FSharpLint.Framework
open FSharpLint.Framework.Ast
open FSharpLint.Framework.Configuration
open FSharpLint.Application

[<AllowNullLiteralAttribute>]
type FsLintRule(name : string, value : string) =
    member val Rule : string = name with get
    member val Text : string = value with get

type SonarRules() = 

    let fsLintProfile = 
        let resourceManager = new ResourceManager("Text" ,Assembly.Load("FSharpLint.Framework"))
        let set = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true)
        let mutable rules = List.Empty
        
        for resoure in set do
            let lem = resoure :?> DictionaryEntry
            try
                if (lem.Key :?> string).StartsWith("Rules") ||
                   (lem.Key :?> string).Equals("LintError")  ||
                   (lem.Key :?> string).Equals("LintSourceError") then
                    let rule = new FsLintRule(lem.Key :?> string, lem.Value :?> string)
                    rules <- rules @ [rule]
            with
            | _ -> ()       
        rules

    member this.GetRule(txt : string) =
        
        let VerifyIfExists(rule : FsLintRule, txtdata : string) = 
            if rule.Text.StartsWith("{") then
                if rule.Text.Contains("can be refactored") && txt.Contains("can be refactored into") then
                    true
                elif rule.Text.Contains("s should be less than") && txt.Contains("s should be less than") then
                    true
                else
                    false
            else
                let start = rule.Text.Split('{').[0]
                if txt.StartsWith(start) then
                    true
                else
                    false


        let foundItem = fsLintProfile |> Seq.tryFind (fun c -> VerifyIfExists(c, txt))
        match foundItem with
        | Some v -> v
        | _ -> null

    member this.ShowRules() =
        fsLintProfile |> Seq.iter (fun c -> printf "%s  = %s\r\n" c.Rule c.Text)
        printf "\r\n Total Rules: %i\r\n" fsLintProfile.Length

type private Argument =
    | ProjectFile of string
    | SingleFile of string
    | Source of string
    | UnexpectedArgument of string

type FsLintRunner(notificationManager : INotificationManager, configuration : FSharpLint.Framework.Configuration.Configuration) =
    let rules = new SonarRules()
    let mutable notsupportedlines = List.Empty
    let mutable issues = List.empty
    let mutable filePath = ""

    let reportLintWarning (warning:FSharpLint.Application.LintWarning.Warning) =
        let output = warning.Info + System.Environment.NewLine + LintWarning.getWarningWithLocation warning.Range warning.Input
        let rule = rules.GetRule(warning.Info)
        if rule <> null then
            let issue = new Issue(Rule = "fsharplint:" + rule.Rule, Line = warning.Range.StartLine, Component = filePath, Message = warning.Info)
            issues  <- issues @ [issue]  
        else
            notsupportedlines <- notsupportedlines @ [output]
            
    let outputLintResult = function
        | LintResult.Success(_) -> notificationManager.ReportMessage(new Message ( Id = "FsLintRunner", Data= "FSharp Link Ok"))
        | LintResult.Failure(error) -> notificationManager.ReportMessage(new Message ( Id = "FsLintRunner", Data= "FSharp Link Error: " + error.ToString()))

    let runLintOnFile pathToFile =
            let parseInfo =
                {
                    FinishEarly = None
                    ReceivedWarning = Some reportLintWarning
                    Configuration = Some configuration
                }

            lintFile parseInfo pathToFile
        
    member this.ExecuteAnalysis(item : VsFileItem) =
        issues <- List.Empty
        filePath <- item.FilePath
        if File.Exists(filePath) then
            runLintOnFile filePath (Version(4, 0)) |> outputLintResult
            notificationManager.ReportMessage(new Message(Id = "FSharpLintRunner", Data = "Issues Found: " + issues.Length.ToString()))
        else
            notificationManager.ReportMessage(new Message(Id = "FSharpLintRunner", Data = "File not found : " + filePath))
        issues


type FSharpLintAnalyser(notificationManager : INotificationManager) =     
    let mutable profile : System.Collections.Generic.Dictionary<string, Profile> = null

    member val Issues : Issue List = List.empty with get, set

    member this.RunLint(item : VsFileItem) = 
        let extension = Path.GetExtension(item.FilePath).ToLower()
        if extension.Equals(".fs") ||   extension.Equals(".fsi") then
            try
                let lintRunner = new FsLintRunner(notificationManager, InputConfigHandler.CreateALintConfiguration(profile))                
                lintRunner.ExecuteAnalysis(item)
            with
            | ex -> notificationManager.ReportMessage(new Message(Id = "FSharpLintRunner", Data = "Failed to run static analysis: " + ex.Message))
                    notificationManager.ReportException(ex)
                    List.Empty
        else
            List.Empty

    member x.UpdateProfile(profileIn : System.Collections.Generic.Dictionary<string, Profile>) =
        profile <- profileIn