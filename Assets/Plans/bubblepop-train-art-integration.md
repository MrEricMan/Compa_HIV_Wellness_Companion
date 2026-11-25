# Current Approved-Plan Revision: Lobby Day Progression

The earlier train-art section below is retained as historical context, not work to repeat. The active requested work is the **Lobby Day 1 → Kai follow-up → Lobby Day 2** extension at the end of this document. No new train-art or phone-framing changes are in scope.

# Project Overview
- **Game Title:** Compa HIV Wellness Companion — BubblePop train minigame (project name inferred from its folder; no renaming).
- **High-Level Concept:** Preserve the existing train-stopping minigame while replacing its geometric placeholders with the supplied train artwork. Players shoot webby gunk off the train before reaching the end of the track.
- **Players:** Single player.
- **Inspiration / Reference Games:** Existing BubblePop scene is the functional and composition reference; no new external reference required for this art-only task.
- **Tone / Art Direction:** Match the supplied TrainSprites images and retain the current NPC and interface.
- **Target Platform:** WebGL; Unity 6000.3.19f1.
- **Screen Orientation / Resolution:** Existing landscape layout; Train HUD reference resolution is 1280×720. Check other landscape aspect ratios without changing global display settings.
- **Render Pipeline:** URP, current PC_RPAsset; retain Sprite-Unlit-Default for gameplay sprites.

## Confirmed Scope
User selected:
1. WebbyGunk pieces are different static gunk variants, randomly chosen as bubble replacements, not animation frames.
2. Tile the existing Track.png; do not generate replacement rail art under this approval.
3. Use all five TrainSprites images, keeping the NPC, UI, shooting and game rules unchanged.

# Game Mechanics
## Core Gameplay Loop
Keep existing pointer aiming, leftward shots, randomly populated 8×4 target grid, round progression, inactivity fallback, distance-based loss, final clear-to-win condition and return/progress recording. Only target visuals become gunk variants.

Important actual scene settings override script defaults: track distance is 350, four rounds are configured, target spacing is 0.78, and adjacent popping is disabled. Preserve these serialized values, quotas, limits, intervals, projectile appearance and audio.

## Controls and Input Methods
Project supports both input backends. The active TrainMinigameController uses event-driven Input System mouse and touchscreen actions; preserve them and the existing UI hit blocking. Legacy BubbleShooterScript on NPC is disabled and must remain disabled. Do not add new controls or enable legacy spawning/shooting components.

# UI
No UI redesign or font changes. Keep pause/settings, distance bar, speed text and result/retry/return panels and bindings unchanged.

Composition reference (not a proposed UI replacement):
```text
+---------------------------------------------------+
| Existing title / distance / speed       Pause      |
|       Passing warning signs, away from HUD         |
| [Train + gunk target grid]      [NPC on handcar]    |
| ================= tiled railway ================= |
| Existing instructions                             |
+---------------------------------------------------+
```
Place signs so their embedded artwork is readable without masking targets, firing position or HUD. Remove/disable the duplicate world-space warning text only within the replaced signs if the supplied image already provides the message.

# Key Asset & Context
## Existing assets to modify
- `Assets/Scenes/BubblePopMinigameScene.unity`: replace visual placeholders and wire imported sprites; preserve gameplay and interface references.
- `Assets/Art Assets/TrainSprites/Train.png`: 1024×512, currently Default texture with no sprites. Import as Single sprite; use as the train body visual, removing/disabling the geometric body/roof/cab/window/wheels that it replaces.
- `Assets/Art Assets/TrainSprites/RailwayHandcar.png`: 512×256, currently Default. Import as Single; replace handcar deck/wheels while retaining NPC and firing transform.
- `Assets/Art Assets/TrainSprites/NoTrackAheadSign.png`: 256×256, currently Default. Import as Single; replace both passing warning boards and redundant text where appropriate.
- `Assets/Art Assets/TrainSprites/Track.png`: 512×128, currently Default. Import as Single, Full Rect mesh; repeat at uniform scale to avoid stretching the artwork.
- `Assets/Art Assets/TrainSprites/WebbyGunk.png`: 1024×256, currently Default. Import as Multiple and slice separate static variants. The image dimensions suggest four 256×256 cells, but the actual visible boundaries/transparent margins must be checked before committing those rectangles. Use deterministic names such as WebbyGunk_0… and centered pivots; do not invent additional variants.
- `Assets/Scripts/BubblePop/TrainMinigameController.cs`: only minimal presentation integration for tiled track motion/reset and, if necessary, consistent gunk sizing; retain mechanics and input.

## Proposed new asset
- `Assets/Scripts/BubblePop/TrainTrackScroller.cs`: dedicated tiled-track presentation component, only if existing passing-object movement cannot safely support seamless track wrapping. Tiles repeat at a shared world width; wrap span is tile-count × tile-width, distinct from the existing warning-sign wrap range.

## Important implementation context
- Active scene has geometric train, handcar, track, 22 passing sleepers and two signs. `_passingObjects` currently holds the sleepers and signs; update its bindings to remove retired sleepers and retain the signs.
- Active controller creates targets directly using `_bubbleSprites` in `SpawnRandomBubble()` and sizes them through `MakeSprite(...)`. Assign gunk to this scene array, NOT merely to the legacy Bubble prefab.
- Preserve `_projectileSprite` (BubbleSpriteSheet_26), NPC art (NpcIdle1.png.png), `_spriteMaterial`, grid location/hit behavior and world-space layer order unless a small visual alignment adjustment is necessary.
- Runtime target size uses sprite bounds; padded cells can make visible gunk too small. Verify visible content and fit it comfortably in each existing cell without changing hit radius or spacing.
- `Assets/Prefabs/BubblePopPrefabs/Bubble.prefab` and `Projectile.prefab`, original BubbleSpriteSheet and shared UI/pause prefabs are not in scope for edits.
- Read-only dependency inspection found no scene/prefab references to the five TrainSprites textures yet. TrainMinigameController is referenced by this minigame scene. Recheck impact before execution using the dependency tool if available, otherwise AssetDatabase dependency/dependent queries.
- No existing plan was found: `Assets/Plans/` did not exist during inspection.
- This is sprite-sheet slicing and direct sprite assignment, not a SpriteAtlas packaging change. No Addressables, atlas generator, new packages or packing settings are required.

# Implementation Steps
1. **Validate source art and impact.**
   - Inspect all five images visually, including alpha, orientation, actual gunk boundaries and Track.png edge continuity.
   - Recheck dependencies/dependents for all edited assets and scripts before modifications; preserve asset GUIDs by editing importers in place rather than replacing source files.
   - Confirm the loaded scene has no unrelated unsaved changes before saving.
   - **Dependencies:** Approval of this plan.

2. **Import the five images and slice gunk.**
   - Use TextureImporter settings for Single/Multiple sprites and Sprite Editor data-provider APIs for slicing when available; never hand-edit .meta files.
   - Set alpha handling appropriate to the supplied files, consistent filtering/PPU, and retain full source resolution without unnecessary CPU Read/Write access.
   - Verify whether 4×1 slicing matches actual content; create one sub-sprite per supplied design and stable names/IDs. If boundaries differ, use accurate rectangles rather than blind grid assumptions.
   - Reimport and confirm all resulting sprites are valid.
   - **Files:** Five TrainSprites PNG import settings/sub-sprite metadata only; no source image redraw.
   - **Dependencies:** Step 1.

3. **Replace train, handcar and signs in the scene.**
   - Use the train and handcar at uniform scale with wheels aligned to the track; reset placeholder-specific tints to white.
   - Disable/remove only the superseded geometric SpriteRenderers/visual children. Keep needed parent transforms and all controller/gameplay objects intact.
   - Maintain the train behind the gunk and handcar behind the NPC. Fit the visual train to the existing grid rather than rebalancing gameplay.
   - Replace both passing boards with NoTrackAheadSign.png, adjusting position/scale and hiding duplicate text when the image already contains it.
   - **Files:** BubblePopMinigameScene.unity.
   - **Dependencies:** Step 2.

4. **Build a repeating track from existing Track.png.**
   - Replace the stretched blank rail and individual placeholder sleepers with repeated full Track.png tiles at uniform scale.
   - Cover the orthographic camera view plus offscreen wrap padding; tile spacing must equal the sprite's world width. Check wheel baseline and seams.
   - Retain existing sign motion. Remove retired sleepers from `_passingObjects` so no missing references remain.
   - If needed, introduce TrainTrackScroller with a track-specific wrap span; call it from the active controller's existing unpaused Update using the same round train speed × scenery scale. Reset it from RestartGame. It must not independently move during pause/end or alter round speed.
   - Keep scrolling at all four configured round speeds. Do not reuse the fixed 22-unit sign wrap span unless it is mathematically compatible with the tile strip.
   - If Track.png has unavoidable artwork seams, report the issue for a separate art decision rather than silently remaking the rail.
   - **Files:** BubblePopMinigameScene.unity; optionally TrainTrackScroller.cs and minimal TrainMinigameController.cs integration.
   - **Dependencies:** Steps 2–3.

5. **Wire gunk into live target spawning.**
   - Set Train Challenge `_bubbleSprites` to all sliced WebbyGunk variant sprites in a stable order. Keep random variant selection and projectile sprite unchanged.
   - Check target visibility and bounds at 0.78 cell spacing, retaining existing hit testing and single-target popping.
   - Prefer import rectangles/pivots and scene assignments over controller changes. If source margins require a visual size adjustment, scope it to gunk targets only, not projectile scaling.
   - **Files:** BubblePopMinigameScene.unity; TrainMinigameController.cs only if unavoidable for presentation sizing.
   - **Dependencies:** Steps 2–4.

6. **Verify and save the integration.**
   - Run the checks below, inspect console, and compare serialized gameplay values with the pre-edit state.
   - Save only the intended scene/assets and report the actual changed files and any unresolved visual limitation.
   - **Dependencies:** Steps 3–5.

# Verification & Testing
## Asset/import checks
- All five supplied images are imported as usable sprites and appear in the minigame.
- Each gunk sub-sprite isolates one complete design, without clipping/neighbour bleed, with stable names and usable pivots.
- Train/handcar/signs are not distorted or accidentally tinted; no duplicated geometric placeholder art remains visible.
- Source PNGs/GUIDs remain intact; other scenes and legacy prefabs are unchanged.

## Scene/runtime checks
- Fresh start spawns visible random gunk variants, not the old bubbles; enough sampling demonstrates every supplied variant can spawn.
- NPC and firing point remain correctly aligned with the handcar; leftward projectiles still hit intended targets and UI clicks do not fire.
- Gunk stays readable on the train with correct draw order; signs do not obscure HUD or targets.
- Track is gap-free over multiple wraps at every round speed; tiles do not stretch, jump, overlap incorrectly or expose old sleepers.
- Pause freezes both track/sign motion and gameplay; resuming continues cleanly. Loss/win freezes scenery as before. Retry restores track and warning positions without drift/duplicate tiles.
- Preserve four rounds, track distance 350, spacing 0.78, adjacent popping disabled, inactivity fallback, quotas, audio, win recording and return behavior.
- Exercise lose/retry and win/return paths with existing controls; no console errors or missing references.

## Layout and WebGL checks
- Inspect Game view at 1280×720, 1920×1080 and a wider landscape aspect ratio; camera coverage and HUD/target readability remain acceptable.
- Perform a WebGL build/smoke test if the installed build support and environment allow it; otherwise explicitly report that browser verification remains pending.
- No unit-test framework installation is required for this visual integration. If an existing test setup is available, add targeted track-wrap/reset assertions; otherwise use deterministic manual wrap/reset checks.

---

# Lobby Day Progression — Active Implementation Extension

# Project Overview
- **Game Title:** Compa HIV Wellness Companion.
- **High-Level Concept:** On Day 1 an otherwise empty lobby introduces Kai's room visit through Compa. Winning Kai's train minigame and finishing the room follow-up begins Day 2 in the lobby, where Kai replaces Compa and can be spoken to through the existing dialogue presentation.
- **Players:** Single player.
- **Inspiration / Reference Games:** The project's existing NPCInteractionScene is the interaction/dialogue reference; no external redesign.
- **Tone / Art Direction:** Preserve lobby environment, existing Compa/Kai sprites, dialogue styling and friendly guidance.
- **Target Platform:** WebGL, including mouse and phone touch; Unity 6000.3.19f1.
- **Screen Orientation / Resolution:** Existing landscape layouts; validate 16:9 and wide-phone landscape aspect ratios.
- **Render Pipeline:** Existing URP PC_RPAsset; no rendering-pipeline changes.

## Confirmed Requirements
- Day 1: only the visible Compa world NPC is present; Kai and the already-hidden Companion/Player world objects remain absent. UI labels such as RSVP attendee names are not world NPCs and are not removed.
- Dialogue opens only by tapping/clicking the NPC, not automatically on lobby entry.
- Compa's exact Day 1 line: “It looks like no one is here! But I hear that Kai has something to say. Head to the elevator to go to their room.”
- Keep the route LobbyScene → Elevator → NPCInteractionScene → BubblePopMinigameScene → NPCInteractionScene post-win dialogue.
- Finish Kai's post-win dialogue, then advance to Day 2 and load LobbyScene.
- Day 2: Compa disappears, Kai appears. Kai's lobby lines are explicitly Inspector-editable placeholders.
- Session-only progression: no new local/cloud save, PlayerPrefs writes, persistence UI or reset menu.

# Game Mechanics
## Core Gameplay Loop
Enter the lobby → tap Compa for guidance → use the existing elevator and Kai room button → talk to Kai and play the existing train minigame → finish the post-win room conversation → return to the changed Day 2 lobby → tap Kai to read editable daily dialogue. Keep failure/retry and room invitation behavior intact. Narrative closure before the day change follows the user's selected approach and avoids losing the existing post-win beat.

## Controls and Input Methods
Use the active InputSystemUIInputModule and event-driven pointer events for both mouse and touch. Reuse the currently empty EventClick.cs as an opt-in world-NPC click adapter implementing IPointerClickHandler and exposing an Inspector UnityEvent. Configure sprite-aligned 3D BoxCollider hit targets and a PhysicsRaycaster on the actual lobby gameplay camera, because this is a 3D lobby with sprite NPCs. Do not use legacy OnMouseDown or polling. Inspect which camera renders the lobby before wiring its raycaster; two camera objects exist.

UI must take priority: clicking Continue/Close, pause, elevator or community-board controls must not also hit a world NPC. Gate opening during modal pause/settings/community panels and avoid duplicate input handling. Existing EventClick usages with an empty event remain inert.

# UI
Reuse the existing NPCInteractionScene dialogue hierarchy and art as a scene-local lobby copy. Do not build a new dialogue design or change shared pause/RSVP/Compa-button prefabs. Preserve the room UI and its callbacks.

```text
Lobby environment
  Day 1: [Compa clickable]              [Elevator]
  Day 2: [Kai clickable]                [Elevator]

On NPC tap:
+-----------------------------------------------+
| NPCName: COMPA or KAI                          |
| Dialogue text                                 |
|                                Continue  Close|
+-----------------------------------------------+
```
Use one shared lobby panel with source-specific dialogue data and an active-source router in LobbyDayController. Continue/Close forward to the selected DialogueTriggers source; selecting a different NPC closes the previous source first. Speaker label is refreshed when selecting Compa/Kai. Put source components and the day controller on always-active objects so hiding an NPC or dialogue panel does not disable the progression subscriptions or lose references.

# Key Asset & Context
## Verified existing systems
- `Assets/Scripts/GameProgressManager.cs`: session singleton initialized before scene load; CurrentDay starts at 1. RecordWin adds a pending follow-up without advancing the day. FinishFollowUp removes it, increments CurrentDay once and emits DayChanged. Reuse without save-system changes.
- `Assets/Scripts/DialogueTriggers.cs`: functional source/proxy conversation handling; PlayerClick, ContinueButton and ExitButton are public. DayDialogue exposes editable day/lines. Its current Day 1 branch always offers a minigame and its Start can auto-open a pending follow-up: lobby use therefore needs an explicit opt-in dialogue-only mode, not just assignment of the text field.
- `Assets/Scripts/EventClick.cs`: currently an empty component with UnityEngine.InputSystem imported. Extend as the configured click adapter rather than inventing another input framework.
- `Assets/Scenes/NPCInteractionScene.unity`: actual dialogue source has the current Kai invitation/post-win lines; `_nextDayScene` is currently APTCenterScene. Existing proxy components point at the source. Change only the source's serialized next-day destination to LobbyScene.
- `Assets/Scenes/LobbyScene.unity`: currently contains active world Compa and Kai, inactive Companion and Player, InputSystemUIInputModule, pause/community/RSVP UI, and an Elevator button. No lobby DialogueTriggers or NPC colliders are presently connected.
- `Assets/Scripts/ElevatorSceneChange.cs`, `ElevatorUIScript.cs`, `LobbyButtons.cs`: existing scene navigation wrappers. The lobby Elevator button routes to Elevator; the elevator KaiButton routes to NPCInteractionScene. Preserve these.
- All required route scenes are enabled in EditorBuildSettings; no build-list change is initially needed.
- Dependency/dependent checks were done through AssetDatabase because the dedicated dependency tool is unavailable in the current tool set. DialogueTriggers/EventClick currently have NPCInteractionScene as a scene dependent; check again before changing source code.

## Files to modify/create
1. `Assets/Scripts/DialogueTriggers.cs`: add a default-off serialized dialogue-only option. In this mode use the existing day/lines array even on Day 1, never auto-open a pending follow-up, never offer/launch a minigame and never consume/advance global progress. Preserve default room behavior and all existing public button signatures; add only any small API required for explicit close/source routing.
2. `Assets/Scripts/EventClick.cs`: add opt-in pointer callback and UnityEvent as described above; empty/unconfigured use does nothing. Do not silently activate any old room click component or replace its existing UI-button route.
3. **New:** `Assets/Scripts/LobbyDayController.cs`: serialized Compa/Kai roots, conversation sources, shared panel/speaker label and relevant modal references. Read CurrentDay on initialization and subscribe/unsubscribe DayChanged. Show Compa for day 1 and Kai for day 2 and later as a conservative fallback; additional future-day scheduling is outside scope. Public click handlers select/open the appropriate source, and Continue/Close route to it. Close stale dialogue on state change/scene exit. Validate references and prevent hidden NPCs from opening conversations.
4. `Assets/Scenes/LobbyScene.unity`: configure sources, reused dialogue UI, day visibility controller, NPC hit targets and click events, gameplay-camera raycaster, and scene-local UI forwarding.
5. `Assets/Scenes/NPCInteractionScene.unity`: set the current Kai source `_nextDayScene` to `LobbyScene`; retain invitation, post-win text, room day-dialogue data and proxy bindings.
6. Optional test files only if existing test tooling supports them; temporary runtime scripts/screenshots must be cleaned up after verification.

No edits to the train minigame, sprite images, elevator scripts, GameProgressManager or unrelated scenes are expected. If discovery reveals a required scope change, report it instead of silently redesigning those systems.

# Implementation Steps
1. **Snapshot actual state and impact.** Read modified source files afresh; inspect the actual room UI hierarchy and source/proxy buttons before copying; confirm lobby camera, visible NPC bounds, existing raycast layers, modal panels and UnityEvent targets. Check scene dirty state and dependencies/dependents before writes. Preserve GUIDs and all unrelated scene settings. **Dependencies:** Approval of this revised plan.
2. **Extend existing dialogue and pointer adapters.** Implement default-off dialogue-only behavior in DialogueTriggers without changing NPCInteractionScene's normal invitation/follow-up paths. Add opt-in EventClick pointer UnityEvent. Handle missing references/empty lines gracefully; closing ordinary lobby dialogue must never call FinishFollowUp or SceneManager.LoadScene. **Files:** DialogueTriggers.cs, EventClick.cs. **Dependencies:** Step 1.
3. **Add lobby day/router component.** Implement LobbyDayController with initialization, DayChanged lifecycle, visibility, selected-source routing, speaker label and modal gating. Keep progression centralized in GameProgressManager and avoid advancing the day here. **File:** LobbyDayController.cs. **Dependencies:** Steps 1–2.
4. **Configure Day 1/2 lobby presentation.** Make a scene-local copy of the existing dialogue UI, remove room-specific forwarding references from that copy and bind lobby Continue/Close. Configure dialogue-only source A with Day 1 Compa's exact line, source B with Day 2 placeholder line(s). Wire source-specific open handlers through EventClick and collider/raycaster setup. Set a safe authored Day 1 NPC state, with runtime controller applying the session state on every entry. Keep Companion/Player hidden; retain current environment, elevator, pause/community controls and existing Compa UI shortcut. **File:** LobbyScene.unity. **Dependencies:** Steps 2–3.
5. **Connect the selected next-day route.** Change the real Kai conversation source's `_nextDayScene` to LobbyScene, not every proxy/default and not the win-return scene. Winning still returns to NPCInteractionScene; closing its post-win panel early leaves the follow-up pending; final Continue advances once and returns to LobbyScene. **File:** NPCInteractionScene.unity. **Dependencies:** Steps 1 and 4.
6. **Verify and hand off.** Run the tests below, inspect console and render the dialogue in both lobby states. Save only intended files. Report where the editable Kai lobby lines live in the Inspector and any unverified physical-browser/touch tests. **Dependencies:** Steps 4–5.

# Verification & Testing
- Fresh session/open LobbyScene: CurrentDay=1, Compa active, Kai inactive, other world NPCs absent and dialogue closed.
- Mouse click and touchscreen tap on Compa both open the reused panel with the exact guidance and correct speaker. Continue on its last line closes only; repeated interactions do not replace guidance with the room invitation, launch a minigame, or advance a day.
- UI/modal tests: Continue/Close each process once; taps on dialogue/pause/settings/community/elevator controls do not trigger NPC interaction behind them. Pause/modal state cannot open lobby conversations.
- Existing route: Lobby Elevator → Elevator scene → KaiButton → NPCInteractionScene remains functional. Kai's invitation still starts BubblePop; loss/retry leaves Day 1 and does not reveal lobby Kai.
- Winning: RecordWin leaves Day 1 with a pending follow-up; return loads NPCInteractionScene and its current post-win conversation auto-opens as before.
- Early Close/reopen preserves pending follow-up. Final Continue calls FinishFollowUp once, makes CurrentDay=2, clears the pending follow-up and loads LobbyScene. Repeated completion cannot advance to Day 3.
- Day 2 entry: Compa inactive, Kai active, panel initially closed; click/tap Kai opens the configured editable placeholder without minigame invitation or automatic scene transition. Change the Inspector line and confirm displayed text matches it.
- Re-enter lobby during the same session: Day 2 visibility and dialogue still work. Restarting Play Mode or a fresh browser session returns to Day 1; no PlayerPrefs or cloud save changes occur.
- Null/empty dialogue data and missing setup references produce useful diagnostics, not null-reference cascades. A fresh scene opened directly in the Editor initializes the session manager correctly.
- Regression: room source/proxy callbacks still use the same conversation; minigame mechanics/presentation and unrelated pause/RSVP/shared prefab assets are unchanged.
- Visual checks: speaker/body/Continue/Close readable and unclipped at 16:9 and wide-phone landscape layouts, no duplicate panel, no invisible raycast overlay blocking world taps while the dialogue is closed.
- Run automated Play Mode scene-transition/input checks if runtime tooling is available, otherwise explicitly mark these manual checks pending. Never claim physical touchscreen or WebGL browser validation from EditMode-only evidence. Build smoke test only if installed WebGL support/environment allows it.

