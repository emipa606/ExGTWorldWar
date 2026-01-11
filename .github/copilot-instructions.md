# GitHub Copilot Instructions for ExGT WorldWar (Continued)

## Mod Overview and Purpose

The ExGT WorldWar (Continued) mod aims to enrich the gameplay of RimWorld by introducing a range of experimental equipment, uniforms, mechanoid bosses, and weaponry inspired by World War I. This continuation of 小面包s' mod is updated with significant new features and improvements to enhance player experience.

## Key Features and Systems

- **Experimental Equipment and Weapons**: Includes new guns, protective gear, and mechanoids that bring the tactical nuances of World War I to RimWorld.
- **Bayonet Charge Mechanism**: Adds a new combat feature enabling pawns to launch bayonet charges against enemies.
- **Call of the Dead**: Introduces supernatural or necromantic elements, potentially invoking undead-themed events or units.
- **White Phosphorus Smoke Grenade**: A new type of grenade that creates a smoke screen with additional effects.
- **Adrenaline Injection**: Introduces a new consumable item allowing pawns to experience temporary boosts, potentially in combat or work efficiency.
- **CE Compatibility**: The mod is designed to be float-compatible with Combat Extended (CE), enhancing tactical gameplay.

## Coding Patterns and Conventions

- **Class Naming**: Classes and methods are named to clearly reflect their purpose, using PascalCase for class names and camelCase for method names.
- **File Structure**: The mod utilizes multiple files to segment features and functionalities logically. Examples include `BDPMod.cs` for core mod functionalities and `PatchMain.cs` for patch processing.
- **Method Implementation**: Emphasis on method separation for clarity and maintainability, such as `Check_DashAndStop()` in `PawnJumper_Dash.cs`.

## XML Integration

- **DefModExtension**: XML files might extend RimWorld's in-built definitions using classes like `TheDead_DefModExtension`. These are crucial for introducing new game definitions associated with mod assets and mechanics.
- **TheDead.cs**: Interacts with XML to define specific game objects or events related to undead elements.

## Harmony Patching

- **Usage**: The mod uses Harmony for detouring and patching original RimWorld methods to extend or modify their functionalities.
- **Patch Classes**: Includes classes like `MYDE_Harmony_ExGTWorldWar_CanTakeOrder` to handle specific patching scenarios.
- **Internal Classes**: Internal classes (e.g., `MYDE_Harmony_ExGTWorldWar_DamageWorker_Extinguish`) are used for Harmony patches, ensuring encapsulation and minimal interference with the base game code.

## Suggestions for Copilot

- **Code Documentation**: Ensure methods and classes have XML-style comments to improve readability and assist Copilot in generating relevant code suggestions.
- **Leverage Harmony**: Provide detailed before-and-after scenarios in Harmony patches for better Copilot understanding.
- **Utilize Pattern Recognition**: Emphasize stated coding patterns and naming conventions consistently across the mod to improve Copilot auto-completion accuracy.

By following these guidelines and understanding the key aspects of the mod, developers can effectively leverage GitHub Copilot to enhance the development of the ExGT WorldWar mod and ensure a clean, efficient codebase.
