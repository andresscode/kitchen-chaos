# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Kitchen Chaos is a Unity 6 cooking game project. It is in early development — currently a fresh project setup with no custom game scripts yet.

## Unity Version & Render Pipeline

- **Unity**: 6000.5.8f1 (Unity 6)
- **Render Pipeline**: Universal Render Pipeline (URP) 17.5.0
- **Input**: New Input System 1.20.0 (configured via `Assets/InputSystem_Actions.inputactions`)

## Key Packages

- `com.unity.inputsystem` 1.20.0 — use `InputAction` / `PlayerInput` component, not legacy `Input.GetKey()`
- `com.unity.render-pipelines.universal` 17.5.0 — shaders must target URP; do not use Built-in RP shaders
- `com.unity.ai.navigation` 2.0.14 — NavMesh for AI pathfinding
- `com.unity.ugui` 2.5.0 — UI Toolkit and uGUI available
- `com.unity.timeline` 1.8.12 — cutscene/sequence authoring
- `com.unity.test-framework` 1.7.0 — Unity Test Runner (EditMode and PlayMode tests)

## Running Tests

Open Unity Editor → **Window > General > Test Runner**, then run EditMode or PlayMode tests. There is no CLI test runner configured yet.

## Git Conventions

All commit messages must use **Conventional Commits** format:

```
<type>: <short description>

# Types: feat, fix, chore, docs, refactor, test, style, perf
```

Examples: `feat: add player movement controller`, `fix: correct recipe timer off-by-one`, `chore: update packages`

Never add a `Co-Authored-By` trailer to any commit.

## Code Conventions

- Scripts go under `Assets/Scripts/` (to be created as the project grows)
- All MonoBehaviours follow Unity lifecycle (`Awake`, `Start`, `Update`, etc.)
- Prefer `[SerializeField] private` over `public` fields for Inspector exposure
- Use the new Input System event callbacks (`OnMove`, `OnInteract`, etc.) rather than polling `Input` directly
