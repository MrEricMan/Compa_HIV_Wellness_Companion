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
