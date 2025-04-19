// TerminalGame.cs
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TerminalGame : MonoBehaviour
{
    public TMP_Text terminalOutput;
    public TMP_InputField commandInput;

    private string currentState = "start";
    private Stack<string> history = new Stack<string>();
    private Dictionary<string, StoryNode> story = new Dictionary<string, StoryNode>();

    private Coroutine typewriterRoutine;
    private int endingCount = 1;

    void Start()
    {
        commandInput.onSubmit.AddListener(OnCommandSubmitted);
        SetupStory();
        ShowCurrentNode();
    }

    void OnCommandSubmitted(string input)
    {
        SoundEffectsManager.Instance.Play(SoundEffectsManager.Effect.Typing);
        input = input.ToLower().Trim();

        if (input == "restart")
        {
            SoundEffectsManager.Instance.Play(SoundEffectsManager.Effect.Clear);
            currentState = "start";
            history.Clear();
            ClearTerminal();
            ShowCurrentNode();
        }
        else if (input == "back" && history.Count > 0)
        {
            SoundEffectsManager.Instance.Play(SoundEffectsManager.Effect.Back);
            currentState = history.Pop();
            ClearTerminal();
            ShowCurrentNode();
        }
        else if (story.ContainsKey(currentState) && story[currentState].options.ContainsKey(input))
        {
            SoundEffectsManager.Instance.Play(SoundEffectsManager.Effect.Success);
            history.Push(currentState);
            currentState = story[currentState].options[input];
            ClearTerminal();
            ShowCurrentNode();
        }
        else
        {
            SoundEffectsManager.Instance.Play(SoundEffectsManager.Effect.Error);
            terminalOutput.text += $"\n> {input}\nUnknown command.";
        }

        commandInput.text = "";
        commandInput.ActivateInputField();
    }

    void ClearTerminal()
    {
        SoundEffectsManager.Instance.Play(SoundEffectsManager.Effect.Clear);
        terminalOutput.text = "";
    }

    void ShowCurrentNode()
    {
        if (typewriterRoutine != null) StopCoroutine(typewriterRoutine);
        if (story.ContainsKey(currentState))
            typewriterRoutine = StartCoroutine(Typewriter(story[currentState].description));
    }

    IEnumerator Typewriter(string fullText)
    {
        terminalOutput.text = "";
        foreach (char c in fullText)
        {
            terminalOutput.text += c;
            SoundEffectsManager.Instance.Play(SoundEffectsManager.Effect.Typing);
            yield return new WaitForSeconds(0.01f);
        }

        if (currentState.StartsWith("ending"))
        {
            terminalOutput.text += $"\n\nThe End ({endingCount}/10).";
            terminalOutput.text += "\n\nUse the 'back' command to go back to a previous decision.";
            endingCount++;
        }
    }

    void SetupStory()
    {
        // Start
        story.Add("start", new StoryNode(
@"You awaken in front of a glowing terminal. Dust dances in the light of an old monitor.

A message blinks faintly: ""TYPE TO BEGIN.""

Options:
- begin
- restart",
            new Dictionary<string, string> {
                { "begin", "room_intro" },
                { "restart", "start" }
            }));

        // Room Intro
        story.Add("room_intro", new StoryNode(
@"You're in a concrete room with three strange terminals: A, B, and C. A soft hum fills the silence.

Options:
- use terminal a
- use terminal b
- use terminal c",
            new Dictionary<string, string> {
                { "use terminal a", "ending1" },
                { "use terminal b", "hallway" },
                { "use terminal c", "ending2" }
            }));

        // Hallway
        story.Add("hallway", new StoryNode(
@"A mechanical door slides open. A corridor stretches out, lights flickering at uneven intervals.

At the end: a mirror, a ladder hatch, and a cracked wall.

Options:
- mirror
- hatch
- search wall",
            new Dictionary<string, string> {
                { "mirror", "mirror_room" },
                { "hatch", "ladder" },
                { "search wall", "secret_door" }
            }));

        // Mirror Room
        story.Add("mirror_room", new StoryNode(
@"In the mirror, you see yourself... but twisted, like a puppet dangling on strings.

Your reflection lifts a finger. Behind you, a panel slides open.

Options:
- break mirror
- enter panel",
            new Dictionary<string, string> {
                { "break mirror", "ending3" },
                { "enter panel", "secret_door" }
            }));

        // Secret Door
        story.Add("secret_door", new StoryNode(
@"You push a wall near the mirror. It clicks open—revealing a surveillance room.

Monitors show *you* in every room... but in one of them, you're still asleep.

Options:
- smash monitors
- enter hidden corridor",
            new Dictionary<string, string> {
                { "smash monitors", "ending8" },
                { "enter hidden corridor", "lab_room" }
            }));

        // Ladder Branch
        story.Add("ladder", new StoryNode(
@"The ladder creaks under your weight. It descends further than expected.

You see a hatch with a keypad and a slot for... blood?

Options:
- type code
- cut hand",
            new Dictionary<string, string> {
                { "type code", "ending5" },
                { "cut hand", "ritual_room" }
            }));

        // Ritual Room
        story.Add("ritual_room", new StoryNode(
@"A door opens. Inside: chalk symbols and empty robes on the floor.

A single candle flickers. Shadows dance unnaturally.

Options:
- wear robe
- extinguish candle",
            new Dictionary<string, string> {
                { "wear robe", "ending6" },
                { "extinguish candle", "ending7" }
            }));

        // Lab Room
        story.Add("lab_room", new StoryNode(
@"You enter a cold lab. A table displays vials labeled with different emotions.

A terminal glows: 'Choose your reality.'

Options:
- inject fear
- inject curiosity
- inject none",
            new Dictionary<string, string> {
                { "inject fear", "ending9" },
                { "inject curiosity", "ending6" },
                { "inject none", "ending5" }
            }));

        // Optional Fork (not directly linked; you can wire this in as you like)
        story.Add("optional_fork", new StoryNode(
@"A cloaked figure stands under a flickering light.

It offers you a candle or a robe.

Options:
- take candle
- take robe",
            new Dictionary<string, string> {
                { "take candle", "ending7" },
                { "take robe", "ending6" }
            }));

        // ========== ENDINGS ==========
        story.Add("ending1", new StoryNode(
@"You interact with Terminal A. A static burst... then silence.

Your heart stops.", new Dictionary<string, string>()));

        story.Add("ending2", new StoryNode(
@"The screen shows your memories in reverse. You forget who you are.", new Dictionary<string, string>()));

        story.Add("ending3", new StoryNode(
@"In the mirror, you see... yourself. But it’s not you. It speaks first.

You scream. It doesn’t.", new Dictionary<string, string>()));

        story.Add("ending4", new StoryNode(
@"The hatch opens to a spiral staircase descending into a void.

You walk down. You never stop walking.", new Dictionary<string, string>()));

        story.Add("ending5", new StoryNode(
@"You enter a code.

'Access Denied.' A gas floods the chamber.

You smile as it begins.", new Dictionary<string, string>()));

        story.Add("ending6", new StoryNode(
@"You wear the robe. Voices whisper ancient truths in your ears.

Your eyes turn black.", new Dictionary<string, string>()));

        story.Add("ending7", new StoryNode(
@"The candle dies. The room doesn’t go dark… it fades.

You become the shadow.", new Dictionary<string, string>()));

        story.Add("ending8", new StoryNode(
@"You smash the monitors. Sparks fly, and alarms blare.

You’re trapped in a loop. The game begins again.", new Dictionary<string, string>()));

        story.Add("ending9", new StoryNode(
@"The moment you inject fear, the world reshapes.

You are the creature now.", new Dictionary<string, string>()));

        story.Add("ending10", new StoryNode(
@"You unplug the terminal.

Darkness. Forever.", new Dictionary<string, string>()));
    }
}

[System.Serializable]
public class StoryNode
{
    public string description;
    public Dictionary<string, string> options;

    public StoryNode(string desc, Dictionary<string, string> opts)
    {
        description = desc;
        options = opts;
    }
}
