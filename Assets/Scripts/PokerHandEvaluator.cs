using System.Collections.Generic;
using System.Linq;

public static class PokerHandEvaluator
{
    // Each hand type will have a base chip score and a multiplier
    private static readonly Dictionary<string, (int baseChip, int multiplier)> handScoring = new()
    {
        { "High Card", (5, 1) },
        { "One Pair", (10, 2) },
        { "Two Pair", (20, 2) },
        { "Three of a Kind", (30, 3) },
        { "Straight", (30, 4) },
        { "Flush", (35, 4) },
        { "Full House", (40, 4) },
        { "Four of a Kind", (60, 7) },
        { "Straight Flush", (100, 8) }
    };

    public static (int baseChip, int multiplier) GetHandScoring(string handType)
    {
        if(handScoring.ContainsKey(handType))
        {
            return handScoring[handType];
        }
        return handScoring[handType];
    }

   public static (string handType, int score) EvaluateHand(List<CardHandler> cards)
    {
        if (cards.Count < 2) return ("", 0); // No valid hand

        var ranks = cards.Select(c => c.rank).OrderBy(r => r).ToList();
        var suits = cards.Select(c => c.suit).ToList();
        bool isFlush = suits.Distinct().Count() == 1;
        bool isStraight = IsStraight(ranks);
        
        string handType = "High Card"; // Default hand

        // Determine the hand type
        if (isStraight && isFlush) handType = "Straight Flush";
        else if (HasMatchingRanks(ranks, 4)) handType = "Four of a Kind";
        else if (HasFullHouse(ranks)) handType = "Full House";
        else if (isFlush) handType = "Flush";
        else if (isStraight) handType = "Straight";
        else if (HasMatchingRanks(ranks, 3)) handType = "Three of a Kind";
        else if (HasTwoPairs(ranks)) handType = "Two Pair";
        else if (HasMatchingRanks(ranks, 2)) handType = "One Pair";

        // Get the base chip score and multiplier for the hand type
        var (baseChip, multiplier) = handScoring[handType];

        // Calculate the score for face value chips (sum of ranks)
        int faceValueChips = ranks.Sum(); // Add up the face values of the cards

        // Special handling for "One Pair" where the pair should add the same value twice
        if (handType == "One Pair" && ranks.Count == 2) 
        {
            faceValueChips = ranks[0] * 2; // Pair value is counted twice
        }

        // Final score calculation: (faceValueChips + baseChip) * multiplier
        int finalScore = (faceValueChips + baseChip) * multiplier;

        return (handType, finalScore);
    }


    public static bool IsStraight(List<int> ranks)
    {
        if (ranks.Count != 5) return false; // Must have exactly 5 cards for a straight

        for (int i = 0; i < ranks.Count - 1; i++)
        {
            if (ranks[i] + 1 != ranks[i + 1])
                return false; // Cards must be consecutive
        }

        return true;
    }

    public static bool HasMatchingRanks(List<int> ranks, int count)
    {
        return ranks.GroupBy(r => r).Any(g => g.Count() == count);
    }

    public static bool HasFullHouse(List<int> ranks)
    {
        var groups = ranks.GroupBy(r => r).Select(g => g.Count()).ToList();
        return groups.Contains(3) && groups.Contains(2);
    }

    public static bool HasTwoPairs(List<int> ranks)
    {
        return ranks.GroupBy(r => r).Count(g => g.Count() == 2) == 2;
    }
}
