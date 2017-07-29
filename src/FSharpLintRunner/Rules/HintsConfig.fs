﻿module HintsConfig

open FParsec
open FSharpLint.Framework.HintParser

open FSharpLint.Rules.HintMatcher
open FSharpLint.Framework.Configuration

open VSSonarPlugins.Types

let parseHints hints =
    let parseHint hint =
        match CharParsers.run phint hint with
        | FParsec.CharParsers.Success(hint, _, _) -> hint
        | FParsec.CharParsers.Failure(error, _, _) -> failwithf "Invalid hint %s" error

    let hintsData = List.map (fun x -> { Hint = x; ParsedHint = parseHint x }) hints

    { Hints = hintsData; Update = Update.Overwrite }

let SonarConfiguration(config : Profile) =
    let hints = 
        let strintData = ConfHelper.GetValueForString(config, "RulesHintRefactor", "Hints", "")
        if strintData <> "" then
            ConfHelper.GetValueForStringList(config, "RulesHintRefactor", "Hints", List.Empty)
        else
            List.Empty

    Map.ofList 
        [ 
            (AnalyserName, 
                { 
                    Rules = Map.ofList [] 
                    Settings = Map.ofList
                        [
                            ("Enabled", ConfHelper.GetEnaFlagForRule(config, "RulesHintRefactor"))
                            ("Hints", Hints((parseHints hints)))
                        ]
                });
    ]