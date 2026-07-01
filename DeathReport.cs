using System;
using System.Collections.Generic;
using BalaurBohemianBroken.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BalaurBohemianBroken;

public class DeathReport {
    // Special fields:
    // internal bleeding
    // THINGS CREATED: created
    // UNIQUE THINGS CREATED: created_unique
    // DISFIGURATIONS: disfigurations
    // DRUGS CONSUMED: drugs_consumed
    // TIME SLEPT: time_slept
    // EXPERIMENTS MET: experiments_met
    // TRADES: trades_made
    // An aptitude for material gain.
    // name a trader that they maxed the trading with.
    // HUGS GIVEN: hugs_given
    // This is undesired.
    // Why are they so keen on this?
    // Cute.
    // Some respite, at least.
    // Could we include this in conditioning? No.
    // FENTANYL CONSUMED: fent_consumed
    // This may explain recent inventory discrepancies. Medical security will be raised.
    // But when, and why?
    // SHOTS FIRED: bullets_fired
    // I don't recall this being part of their briefing.
    // KILLS: creatures_killed
    // DESTRUCTION: things_destroyed
    // Self harm performed.
    
    // Special behaviours:
    // Operator received a phone call. Experiment ended prematurely.
    // Operator decided experiment was a __failure__. Experiment ended prematurely.
    // Operator decided to end shift early. Experiment ended prematurely.
    // Operator became distressed. Dismissed. Experiment ended prematurely.
    // Operator displayed lack of interest in experiment. Suggested ending experiment prematurely. Accepted.
    // Operator had romantic encounter to attend. Experiment ended prematurely.
    // Operator fell asleep. Experiment ended prematurely.
    // Operator was found naming experiment. Involved individuals have a meeting at shift start tomorrow. Experiment ended prematurely.
    // Operator expressed views contradictory to objective. Required sedation and removal. Experiment ended prematurely.
    // Operator became distracted. Experiment ended prematurely.
    
    // Subject spent significant time complaining about equipment load, yet did nothing to solve this. Is this kleptomanical trend genetic or environmental?
    // Subject was slow. Consistently needed negative reinforcement to proceed. Possibilities: Forgetful. Lazy. Undesirably cautious. Explicitly disregarding objective. Suggest more severe briefing routine.
    // Subject failed to comply with negative reinforcement. We cannot waste resources monitoring ineffective experiments. S., this is your job, please address.
    // Subject progressed with significant efficiency, and significant results. May be anomalous data point. Suggest corroborating results with other departments to identify correlation. 
    // Subject progressed with significant efficiency, yet showed insufficient results. S., both extremes of fear conditioning are statistically ineffective.
    // Subject consumed a light bulb. We have agreed to accept this as an accident. A repeat would be most concerning.
    // A second subject has consumed a light bulb. We may need to analyse their generic palette. Possibly mutated.
    // A third subject has consumed a light bulb. Tests have shown they do not prefer the taste of glass or bulb related materials to generic dog food. Further study needed.
    // A fourth subject has consumed a light bulb. Recommendations were suggested to teach them to place the small part in their mouth instead. However, we are not wasting resources on this.
    // Subjects continue to consume light bulbs at an alarming rate. This goes against basic survival instinct and calls into question our neural sequencing. Recommended we do not record this further.
    
    // Things to fit in:
    // longest distance fallen with no damage
    // longest distance with damage
    // traps destroyed
    // brain damag
    // bandages used
    // chest drains used
    // ores mined
    // 
    // suffered stroke
    // survived stroke
    // restarted heart
    // triggered multiple landmines
    // shot multiple times
    // killed salad
    // killed traders
    // eaten by dune
    // made exclusive recipes
    // listened to music
    // very long run
    // epdas read
    // suffered significant disfiguration
    // 
    // prosthetics added
    public RectTransform deathStats;
    public Dictionary<string, IStat> statsAll;
    public Dictionary<string, IStat> statsSpecial;
    public Dictionary<string, IStat> statsBoring;
    private List<TextMeshProUGUI> linesL;
    private List<TextMeshProUGUI> linesR;
    
    // TODO: Alternate font for notes.
    // You wouldn't think sourcing a font would be so difficult, but it is. Loading one is out of the question,
    // that lead down a long undocumented path.
    // Sourcing an existing font sounds easier, but the fonts I want to use are only referenced on prefab a that I can't
    // instantiate myself, at SurvivorNote.
    
    // Originally tried to get distance between the first two lines using this, but it was wrong. So I manually found a value.
    // __instance.deathStats.GetChild(4).position.y - y_start;
    // The areas for text elements are too long for the lines; this resizes it to fit.
    private static float lineHeight = -36;
	private static float lineShrinkage = 30; 
    private static float fontMult = 0.6f;
    private static string variableColor = "#5f1717";
    private static string sColor = "#00414f";
    private static string halColor = "#4f0041";
    private static string finskyColor = "#5e3616";
    private static int specialStatStartLine = 13;
    private static int specialStatEndLine = 19;
    private static float minimumNoteworthiness = 1;
    
    // Consistent ones that appear on the main menu. We don't check these for if they should appear
    // in the special stats section.
    HashSet<string> statsBoringNames = new() {
        "Age",
        "BonesFractured",
        "CaloriesConsumed",
        "CutsOpened",
        "DeathIndex",
        "Dislocations",
        "EndDate",
        "FluidsConsumed",
        "Infections",
        "Intelligence",
        "LayerDepth",
        "LayerNumber",
        "MentalState",
        "Name",
        "PainSufferedAverage",
        "Resilience",
        "RunTime",
        "Shrapnel",
        "SpecimenID",
        "Strength",
    };
    
    // TODO: Couple notes for if you have no further notes.
    
    public DeathReport(GameObject death_stats_prefab, Dictionary<string, IStat> stats) {
        deathStats = Object.Instantiate(death_stats_prefab, death_stats_prefab.transform.parent).GetComponent<RectTransform>();
        PrepareTextFields();
        SplitStats(stats);
        FillHeaders();
        FillBoringStats();
        FillSpecialStats();
    }
    
    private void PrepareTextFields() {
        TextMeshProUGUI first_line = deathStats.GetChild(3).GetComponent<TextMeshProUGUI>();
        
        // Remove buttons.
        Button return_button = deathStats.GetChild(14).GetComponent<Button>();
        Button copy_to_clipboard_button = deathStats.GetChild(15).GetComponent<Button>();
        Object.Destroy(return_button.gameObject);
        Object.Destroy(copy_to_clipboard_button.gameObject);
        
        // Clear existing text objects
        for (int i = 4; i < 14; i++) {
            Object.Destroy(deathStats.GetChild(i).gameObject);
        }
        
        // Prepare the line we use as a prefab
        first_line.richText = true;
        first_line.fontSize *= fontMult;
        Vector2 offset_max = first_line.rectTransform.offsetMax;
        offset_max.x -= lineShrinkage;
        first_line.rectTransform.offsetMax = offset_max;
        first_line.text = "";
				
        // Spawn more lines.
        linesL = new();
        linesR = new();
        linesL.Add(first_line);
        linesR.Add(Object.Instantiate(first_line.transform, first_line.transform.parent).GetComponent<TextMeshProUGUI>());
        for (int i = 4; i < 23; i++) {
            Transform obj = Object.Instantiate(first_line.rectTransform, first_line.rectTransform.parent);
            Vector3 pos = obj.position;
            pos.y += lineHeight * (i - 3);
            obj.position = pos;
            TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
            linesL.Add(tmp);

            obj = Object.Instantiate(obj, obj.parent);
            linesR.Add(obj.GetComponent<TextMeshProUGUI>());
        }
    }
    
    private void SplitStats(Dictionary<string, IStat> all_stats) {
        statsAll = all_stats;
        statsBoring = new();
        statsSpecial = new();
        foreach (IStat stat in all_stats.Values) {
            if (statsBoringNames.Contains(stat.name))
                statsBoring[stat.name] = stat;
            else
                statsSpecial[stat.name] = stat;
        }
    }

    private void FillHeaders() {
        var title = deathStats.GetChild(0).GetComponent<TextMeshProUGUI>();
        title.richText = true;
        title.text = Locale.GetOther("endscreenstatus") + $"#{Stat("DeathIndex")}";
        deathStats.GetChild(1).GetComponent<TextMeshProUGUI>().text = Stat("EndDate");
        deathStats.GetChild(2).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenunresolved");
    }

    private void FillBoringStats() {
        linesL[0].text = Stat("SpecimenID");
        linesR[0].text = $"<align=\"right\">{Stat("Name")}";
        Object.Instantiate(linesR[0], linesR[0].transform.parent).GetComponent<TextMeshProUGUI>().SetText($"<align=\"center\">AGE: {Stat("Age")}");
        linesL[1].text = "MENTALS: " + Stat("MentalState");
        linesL[2].text = $"RESULTS: {Stat("LayerDepth")}, {Stat("LayerNumber")}, IN {Stat("RunTime")}";
        // linesL[3]
        linesL[4].text = $"CONSUMPTION: {Stat("CaloriesConsumed")}, {Stat("FluidsConsumed")}";
        linesL[5].text = $"QUALITIES: {Stat("Strength")}, {Stat("Resilience")}, {Stat("Intelligence")}";
        // linesL[6]
        linesL[7].text = $"PAIN AVERAGE: {Stat("PainSufferedAverage")}";
        linesL[8].text = $"FRACTURES: {Stat("BonesFractured")}";
        linesL[9].text = $"CUTS/SHRAPNEL/INFECTIONS: {Stat("CutsOpened")}/{Stat("Shrapnel")}/{Stat("Infections")}";
        // lines_l[10].text = $"DAMAGE RECEIVED/RECOVERED: {ColorVar(523)}/{ColorVar(402)}";  // TODO: This.
        // lines[11]
        linesL[12].SetText($"<align=\"center\"><b>FURTHER NOTES</b>");
    }

    private void FillSpecialStats() {
        float total_weight = 0;
        List<float> weights = new();
        List<IStat> candidates = new();
        // TODO: Command to print current noteworthiness
        foreach (IStat stat in statsSpecial.Values) {
            float weight = stat.Noteworthiness();
            if (weight < minimumNoteworthiness)
                continue;
            total_weight += weight;
            candidates.Add(stat);
            weights.Add(weight);
        }

        int line_index = specialStatStartLine;
        while (line_index <= specialStatEndLine && candidates.Count > 0) {
            int index = ExpandedDeathReports.GetRandomWeightedIndex(weights, total_weight);
            IStat candidate = candidates[index];
            total_weight -= weights[index];
            candidates.RemoveAt(index);
            weights.RemoveAt(index);
            // TODO: Try for note.
            linesL[line_index].text = $"{candidate.fieldName}{Stat(candidate)}";
            line_index++;
        }
    }

    private string Stat(string stat_name, bool use_color = true) {
        if (!statsAll.TryGetValue(stat_name, out IStat stat)) {
            ExpandedDeathReports.logger.LogError($"Failed to get stat for death report: {stat_name}");
            return "ERR";
        }

        return Stat(stat, use_color);
    }

    private string Stat(IStat stat, bool use_color = true) {
        try {
            return stat.GetStatReadout(use_color);
        }
        catch (Exception ex) {
            ExpandedDeathReports.logger.LogError($"Failed on GetStatReadout for stat: {stat.name}\nException: {ex}");
            return "ERR";
        }
    }
    
    public static string ColorVar(object variable) {
        return $"<color={variableColor}>{variable}</color>";
    }

    public static string ColorS(object note) {
        return $"<color={sColor}>{note}</color>";
    }

    public static string ColorHal(object note) {
        return $"<color={halColor}>{note}</color>";
    }

    public static string ColorFinsky(object note) {
        return $"<color={finskyColor}>{note}</color>";
    }
}