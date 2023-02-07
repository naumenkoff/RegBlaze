﻿using RegistrySearcher.Helpers;
using RegistrySearcher.Models;
using RegistrySearcher.Services;

var findWhat = ConsoleInteractionHelper.RequestInput("I want to find > ", ConsoleColor.Red).ToLower();
var searchService = new SearchService(findWhat);
var searchMatches = searchService.RunRegistrySearch();

var searchResult = new SearchResult(searchMatches);
ConsoleInteractionHelper.PrintColoredLine(searchResult.GetSerializedSearchMatches(), ConsoleColor.White);
ConsoleInteractionHelper.PrintColoredLine(searchService.GetServiceResultMessage(), ConsoleColor.Green);

if (ConsoleInteractionHelper.RequestKeystroke(
        "Do you want to save the scan result in the desktop directory? (Y -> yes) > ", ConsoleKey.Y, ConsoleColor.Red))
{
    var path = await searchResult.SaveSearchResult();
    ConsoleInteractionHelper.PrintColoredLine($"The result is saved in '{path}'.", ConsoleColor.Green);
}