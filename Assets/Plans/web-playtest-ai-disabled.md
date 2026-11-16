# Project Overview
- Game Title: Compa HIV Wellness Companion (project name).
- High-Level Concept: Existing wellness companion game; prepare its non-AI experience for browser playtesting, with AI deliberately unavailable.
- Players: Existing configuration; no multiplayer changes.
- Inspiration / Reference Games: Not relevant to this compatibility patch; unchanged.
- Tone / Art Direction: Existing art and tone unchanged.
- Target Platform: WebGL, Unity 6000.3.19f1.
- Screen Orientation / Resolution: Preserve current settings and responsive behavior.
- Render Pipeline: URP, current PC_RPAsset.

# Game Mechanics
## Core Gameplay Loop
Preserve existing navigation, scripted NPC interactions and minigames. Inspect progression to ensure no required step waits for a live AI response. AI functionality is explicitly out of scope.
## Controls and Input Methods
Preserve existing controls. Project context reports Both input backends; loaded menu uses InputSystemUIInputModule. Verify PlayerSettings before any input edits; no new controls planned.

# UI
Preserve existing menus and HUD. Avoid visual redesign. In the AI interaction path, suppress recording/thinking/error UI and, if necessary for clarity, reuse existing reply/status presentation for a static 'AI unavailable in this playtest' message. Preserve exit/navigation controls.
Conceptual layout: existing scene and navigation / existing companion area showing unavailable status rather than a talk prompt.

# Key Asset & Context
- Assets/AI Framework/AvatarKit/Runtime/AvatarBrain.cs: verified unchanged microphone references in Start and recording methods. Preserve public class and callback signatures used by other components.
- AvatarMemory.cs, AvatarVoice.cs, AvatarVoiceAPI.cs, AvatarKnowledge.cs and AvatarUI/SpeechToText.cs: inspect lifecycle and external entry points to prevent Web playtest calls from reading credentials, writing AI memory or contacting providers.
- AvatarSpeechBubble.cs: preserve compatibility with LastReply.
- Assets/Scripts/MainMenuFlow.cs, TutorialDialogue.cs, LobbyButtons.cs, MapButtonsScript.cs, DialogueTriggers.cs and minigame return logic: inspect navigation and progression.
- Build configuration: currently eight enabled scenes, with SampleScene first and MainMenu sixth. Inspect active build profile and SampleScene purpose before choosing startup ordering. Do not assume MainMenu is already the launch scene.
- Console currently empty; historical build errors were seven CS0103 Microphone errors. A fresh build is necessary.
- No plan previously existed in Assets/Plans. No project edits made during discovery.

# Implementation Steps
1. Confirm working branch and working-tree state using available tooling; never switch branches or discard user work. Inspect active build profile, PlayerSettings and relevant asset dependencies before editing. Read remaining AI lifecycle methods and scene navigation wiring. Dependencies: none.
2. Apply a minimal Web-only disabled-AI path to AvatarBrain.cs using Unity's built-in platform compilation conditions (never add Unity-generated symbols to project defines). Exclude every unsupported Microphone reference from Web compilation. Bypass Web startup checks, greeting, recording, chat and speech entry points while preserving Editor/native behavior and serialized/public interfaces. Guard Update, public callbacks and context-menu test entry points as needed. Dependencies: Step 1.
3. Prevent any independently running AI memory or voice routines from executing in the Web playtest, based on the lifecycle inspection. Prefer narrow guards over deleting files or disabling entire scene objects that contain navigation. No backend, text-chat implementation, keys or network service changes. Dependencies: Steps 1–2.
4. Inspect reachable scenes and ensure disabled AI cannot block exit or progression. Make only necessary playtest bypasses. Confirm launch scene and scene inclusion in the active build profile; change ordering only if investigation establishes MainMenu is the intended entry. Preserve legacy scene paths and native AI behavior. Dependencies: Steps 1–3.
5. Attempt a development Web build to a dedicated output folder outside Assets. Resolve additional evidenced compilation/build blockers iteratively, staying within project-owned compatibility changes. Do not alter package source or weaken compatibility guards. Report unmet version/module requirements instead. Dependencies: Steps 2–4.
6. Test through a local web server if supported by available tools, inspecting browser console/network and verifying navigation, scripted dialogue, minigames and AI-disabled scene exit. If browser/build tooling is unavailable, clearly report that limitation and provide manual steps rather than claiming verification. Dependencies: Step 5.
7. Summarize changed files, build output, verification results and hosting requirements. Do not deploy publicly or commit changes without authorization. Dependencies: Steps 5–6.

# Verification & Testing
- Fresh Web compilation has no Microphone CS0103 errors.
- Build report succeeds and generated loader/data/wasm files exist; distinguish compilation success from runtime success.
- Intended launch scene opens, tutorial continues/skips, and referenced destinations are included.
- NPC/minigame navigation and returns function without AI responses.
- Entering AI scene produces no missing-key errors, microphone access, transcription/chat/TTS/embedding requests or AI journal writes; exit remains available.
- Public speech callbacks and SayTestLine are harmless in Web.
- Editor/native compilation retains original AI functionality; do not send paid API requests merely to test this.
- Confirm browser hosting preserves generated paths and correct wasm MIME/compression headers. No provider credentials included in output.
- Record all untested routes and any unrelated existing defects; no guarantee of full browser playability until tested.
