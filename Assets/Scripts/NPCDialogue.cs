using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string npcName = "NPC";

    [TextArea(2, 5)]
    public string[] lines;
}