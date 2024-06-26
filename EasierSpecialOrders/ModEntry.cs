﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.GameData;
using StardewModdingAPI;
using StardewValley;
using StardewValley.SpecialOrders;
using StardewModdingAPI.Events;
using System.Linq;
using StardewValley.Objects;

namespace EasierSpecialOrders
{
    public class ModEntry
        : Mod
    {
        private ModConfig config;
        public override void Entry(IModHelper helper)
        {
            this.config = helper.ReadConfig<ModConfig>();

            helper.Events.Content.AssetRequested += this.AssetRequested;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.BuildConfigMenu();
        }

        private void BuildConfigMenu()
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.config)              
            );

            configMenu.AddBoolOption(
                ModManifest,
                name: () => "RemoveCollectionObjective",
                tooltip: () => "Removes the collection objective from two part special orders.",
                getValue: () => config.RemoveCollectionObjective,
                setValue: value => config.RemoveCollectionObjective = value
            );

            configMenu.AddSectionTitle(this.ModManifest, () => "Easier Qi Challenges");

            configMenu.AddBoolOption(
                ModManifest,
                name: () => "Ship Less Qi Fruit",
                tooltip: () => "Only ship 300 qi fruit",
                getValue: () => config.EasierQiChallenges.ShipLessQiFruit,
                setValue: value => config.EasierQiChallenges.ShipLessQiFruit = value
            );
            configMenu.AddBoolOption(
               ModManifest,
               name: () => "Donate Less Prismatic Shards",
               tooltip: () => "Only donate 2 prismatic shards",
               getValue: () => config.EasierQiChallenges.DonateLessPrismaticShards,
               setValue: value => config.EasierQiChallenges.DonateLessPrismaticShards = value
           );
            configMenu.AddBoolOption(
               ModManifest,
               name: () => "Lower Junimo Kart Score",
               tooltip: () => "Only score 25,000 in Junimo Kart",
               getValue: () => config.EasierQiChallenges.LowerJunimoKartScore,
               setValue: value => config.EasierQiChallenges.LowerJunimoKartScore = value
           );
            configMenu.AddBoolOption(
               ModManifest,
               name: () => "Ship Less Cooked Items",
               tooltip: () => "Only ship 50,000g worth of freshly cooked items",
               getValue: () => config.EasierQiChallenges.ShipLessCookedItems,
               setValue: value => config.EasierQiChallenges.ShipLessCookedItems = value
           );
            configMenu.AddBoolOption(
               ModManifest,
               name: () => "Give Less Gifts",
               tooltip: () => "Only give 30 loved gifts",
               getValue: () => config.EasierQiChallenges.GiveLessGifts,
               setValue: value => config.EasierQiChallenges.GiveLessGifts = value
           );
            configMenu.AddBoolOption(
               ModManifest,
               name: () => "More Time For Extended Family",
               tooltip: () => "You have a week to cath the legendary fish family members",
               getValue: () => config.EasierQiChallenges.MoreTimeForExtendedFamily,
               setValue: value => config.EasierQiChallenges.MoreTimeForExtendedFamily = value
           );
            configMenu.AddBoolOption(
               ModManifest,
               name: () => "Less Items For Qis Prismatic Grange",
               tooltip: () => "Only give 50 of each colored item",
               getValue: () => config.EasierQiChallenges.LessItemsForQisPrismaticGrange,
               setValue: value => config.EasierQiChallenges.LessItemsForQisPrismaticGrange = value
           );

        }

        private void AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo($"Data\\SpecialOrders"))
            {
                e.Edit(asset =>
                {
                    var specialorder = asset.Data as Dictionary<string, StardewValley.GameData.SpecialOrders.SpecialOrderData>;

                    /* Remove collection count from special order, not removed from Robin2 
                     * as this would cause the order to be completed immediately with no reward */
                    void RemoveCount(string order)
                    {
                        specialorder[order].Objectives[0].RequiredCount = "0";
                    }

                    if (this.config.RemoveCollectionObjective == true)
                    {
                        RemoveCount("Willy");
                        RemoveCount("Pam");
                        RemoveCount("Pierre");
                        RemoveCount("Robin");
                        RemoveCount("Gus");
                        RemoveCount("Lewis");
                        RemoveCount("Linus");
                        RemoveCount("Evelyn");
                        RemoveCount("Gunther");
                        RemoveCount("Caroline");
                        RemoveCount("QiChallenge4");
                        specialorder["QiChallenge4"].Objectives[0].Text = "";
                        this.Monitor.Log("Removed collection requirements from two part special orders...");
                    }

                    // Create easier requirements for given QiChallenge based on config settings
                    if (this.config.EasierQiChallenges.ShipLessQiFruit == true)
                    {
                        specialorder["QiChallenge2"].Objectives[0].RequiredCount = "300";
                        this.Monitor.Log("Lowered number of qi fruit to ship to 300");
                    }

                    if (this.config.EasierQiChallenges.LowerJunimoKartScore == true)
                    {
                        specialorder["QiChallenge3"].Objectives[0].RequiredCount = "25000";
                        this.Monitor.Log("Lowered required Junimo Kart score to 25,000");
                    }

                    if (this.config.EasierQiChallenges.DonateLessPrismaticShards == true)
                    {
                        specialorder["QiChallenge4"].Objectives[1].RequiredCount = "2";
                        this.Monitor.Log("Lowered prismatic shards to donate to 2");
                    }

                    if (this.config.EasierQiChallenges.ShipLessCookedItems == true)
                    {
                        specialorder["QiChallenge6"].Objectives[0].RequiredCount = "50000";
                        this.Monitor.Log("Lowered value of cooked items to ship to 50,000");
                    }

                    if (this.config.EasierQiChallenges.GiveLessGifts == true)
                    {
                        specialorder["QiChallenge7"].Objectives[0].RequiredCount = "30";
                        this.Monitor.Log("Lowered number of loved gifts to give to 30");
                    }

                    if (this.config.EasierQiChallenges.MoreTimeForExtendedFamily == true)
                    {
                        specialorder["QiChallenge8"].Duration = 0;
                        this.Monitor.Log("Time limit for Extended Family set to one week");
                    }

                    if (this.config.EasierQiChallenges.LessItemsForQisPrismaticGrange == true)
                    {
                        foreach (var objective in specialorder["QiChallenge12"].Objectives)
                        {
                            objective.RequiredCount = "50";
                        }
                        this.Monitor.Log("Lowered number of coloured items to 50 each");
                    }
                });
            }

            else if (e.NameWithoutLocale.IsEquivalentTo($"Strings\\SpecialOrderStrings"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;

                    this.Monitor.Log("Fixing special order strings to show any requirement changes");

                    if (this.config.RemoveCollectionObjective == true)
                    {
                        data["Willy_Objective_0_Text"] = "";
                        data["Willy_Objective_1_Text"] = "Place 100 bug meat in the barrel next to Willy's house.";

                        data["Pam_Objective_0_Text"] = "";
                        data["Pam_Objective_1_Text"] = "Place 12 potato juice in Pam's kitchen.";

                        data["Pierre_Objective_0_Text"] = "";
                        data["Pierre_Objective_1_Text"] = "Place 25 gold-quality vegetables in the empty produce box in Pierre's shop.";

                        data["Robin_Objective_0_Text"] = "";
                        data["Robin_Objective_1_Text"] = "Add 80 hardwood to the stockpile in Robin's house.";

                        data["Gus_Objective_0_Text"] = "";
                        data["Gus_Objective_1_Text"] = "Put 24 eggs in Gus' fridge.";

                        data["Lewis_Objective_1_Text"] = "";

                        data["Linus_Objective_0_Text"] = "";
                        data["Linus_Objective_1_Text"] = "Dump 20 trash in the recycling bin behind the train platform.";

                        data["Evelyn_Objective_0_Text"] = "";
                        data["Evelyn_Objective_1_Text"] = "Place 12 leeks in Evelyn's kitchen.";

                        data["Gunther_Objective_1_Text"] = "";
                        data["Gunther_Objective_0_Text"] = "Place 100 bones in the drop box at the museum counter.";
                    }

                    if (this.config.EasierQiChallenges.ShipLessQiFruit == true)
                    {
                        data["QiChallenge2_Text"] = data["QiChallenge2_Text"].Replace("500", "300");
                        data["QiChallenge2_Objective_0_Text"] = data["QiChallenge2_Objective_0_Text"].Replace("500", "300");
                    }

                    if (this.config.EasierQiChallenges.LowerJunimoKartScore == true)
                    {
                        data["QiChallenge3_Text"] = data["QiChallenge3_Text"].Replace("50,000", "25,000");
                        data["QiChallenge3_Objective_0_Text"] = data["QiChallenge3_Objective_0_Text"].Replace("50,000", "25,000");
                    }

                    if (this.config.EasierQiChallenges.DonateLessPrismaticShards == true)
                    {
                        data["QiChallenge4_Name"] = "Two Precious Stones";
                        data["QiChallenge4_Text"] = data["QiChallenge4_Text"].Replace("4", "2");
                        data["QiChallenge4_Objective_0_Text"] = data["QiChallenge4_Objective_0_Text"].Replace("4", "2");
                    }

                    if (this.config.EasierQiChallenges.ShipLessCookedItems == true)
                    {
                        data["QiChallenge6_Text"] = data["QiChallenge6_Text"].Replace("100,000", "50,000");
                        data["QiChallenge6_Objective_0_Text"] = data["QiChallenge6_Objective_0_Text"].Replace("100,000", "50,000");
                    }

                    if (this.config.EasierQiChallenges.GiveLessGifts == true)
                    {
                        data["QiChallenge7_Text"] = data["QiChallenge7_Text"].Replace("50", "30");
                        data["QiChallenge7_Objective_0_Text"] = data["QiChallenge7_Objective_0_Text"].Replace("50", "30");
                    }

                    if (this.config.EasierQiChallenges.MoreTimeForExtendedFamily == true)
                    {
                        data["QiChallenge8_Text"] = data["QiChallenge8_Text"].Replace("three days", "one week");
                    }

                    if (this.config.EasierQiChallenges.LessItemsForQisPrismaticGrange == true)
                    {
                        data["QiChallenge12_Text"] = data["QiChallenge12_Text"].Replace("100", "50");
                        for (int i = 0; i < 6; i++)
                        {
                            data[$"QiChallenge12_Objective_{i}_Text"] = data[$"QiChallenge12_Objective_{i}_Text"].Replace("100", "50");
                        }
                    }
                });
            }
        }       
    }
}
