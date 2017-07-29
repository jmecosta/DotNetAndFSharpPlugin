﻿module BindingConfig

open FSharpLint.Rules.Binding
open FSharpLint.Framework.Configuration

open VSSonarPlugins.Types

let SonarConfiguration(config : Profile) =
    Map.ofList 
        [ 
            (AnalyserName, 
                { 
                    Rules = Map.ofList 
                        [
                            ("FavourIgnoreOverLetWild", 
                                { 
                                    Settings = Map.ofList 
                                        [ ("Enabled", ConfHelper.GetEnaFlagForRule(config, "RulesFavourIgnoreOverLetWildError")) ] 
                                }) 
                            ("UselessBinding", 
                                { 
                                    Settings = Map.ofList 
                                        [ ("Enabled", ConfHelper.GetEnaFlagForRule(config, "RulesUselessBindingError")) ]
                                }) 
                            ("WildcardNamedWithAsPattern", 
                                { 
                                    Settings = Map.ofList 
                                        [ ("Enabled", ConfHelper.GetEnaFlagForRule(config, "RulesWildcardNamedWithAsPattern")) ] 
                                }) 
                            ("TupleOfWildcards", 
                                { 
                                    Settings = Map.ofList 
                                        [ ("Enabled", ConfHelper.GetEnaFlagForRule(config, "RulesTupleOfWildcardsError")) ]
                                }) 
                        ]
                    Settings = Map.ofList 
                        [
                            ("Enabled", Enabled(true))
                        ]
                });
    ]