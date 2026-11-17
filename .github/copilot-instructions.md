# GitHub Copilot Instructions for ExGT WorldWar (Continued)

## Mod Overview and Purpose
ExGT WorldWar (Continued) is an update and continuation of 小面包's original mod. It introduces various World War I-inspired equipment, uniforms, a mechanoid boss, and specialized weapons. The mod aims to enhance the RimWorld experience with a unique World War I theme, offering players new gameplay mechanics and content.

## Key Features and Systems
### Experimental Equipment
- **Bayonet Charge**: A charge-based combat technique that allows pawns to close the distance with enemies quickly.
- **Call of the Dead**: A skill that summons fallen allies to fight alongside the player.
- **White Phosphorus Smoke Grenade**: A grenade that creates a toxic smoke cloud, damaging enemies and causing them to burn.
- **Adrenaline Injection**: A medical item that temporarily boosts a pawn's stats, enhancing their combat abilities.

### Uniforms
- Various uniforms inspired by World War I, enhancing the visual and thematic aspects of the mod.

### Mechanoid Boss
- A powerful mechanoid boss with unique abilities and behaviors, challenging players to develop new strategies.

### Special Mechanisms
- These include the experimental equipment listed above, providing new gameplay dynamics.

## Coding Patterns and Conventions
- **Mod Structure**: The mod follows the standard RimWorld mod structure, with C# source files and XML definition files.
- **Namespaces**: C# files are organized into namespaces that reflect the mod's structure and functionality.
- **XML Conventions**: XML files use standard RimWorld conventions for defining game elements like apparel, bodies, jobs, and patches.

## XML Integration
- **Def Files**: The mod includes several XML files that define game elements such as apparels, bodies, jobs, and unfinished things.
- **Patches**: XML patches are used to modify existing game definitions and add new content seamlessly.

## Harmony Patching
- **Harmony**: The mod utilizes the Harmony patching framework to modify vanilla RimWorld behavior and integrate new features.
- **Patching Strategy**: Patches are applied to existing game classes and methods to add functionality or override behavior.

## Suggestions for Copilot
- **C# Development**: Provide code snippets for creating new components, defining new game elements, and integrating with existing RimWorld systems.
- **XML Definitions**: Offer examples of XML definitions for apparels, bodies, jobs, and unfinished things.
- **Harmony Patches**: Generate code for creating Harmony patches to modify existing game behavior.
- **Utility Functions**: Suggest utility functions for common tasks such as calculating damage, managing resources, and handling game events.
- **Documentation**: Assist in documenting new features and systems to improve mod usability and maintainability.

## Example XML Definition
xml
<ThingDef>
  <defName>WWI_Bayonet</defName>
  <label>WWI Bayonet</label>
  <description>A sharp bayonet used in close combat.</description>
  <category>Weapon</category>
  <statBases>
    <StatDefOf_Damage>5.0</StatDefOf_Damage>
    <StatDefOf_DamageChance>0.8</StatDefOf_DamageChance>
  </statBases>
</ThingDef>


## Example C# Component
csharp
using RimWorld;
using Verse;

namespace ExGTWorldWar
{
    public class CompProperties_GiveDashGizmos_Equipment : CompProperties
    {
        public CompProperties_GiveDashGizmos_Equipment()
        {
            compClass = typeof(Comp_GiveDashGizmos_Equipment);
        }
    }

    public class Comp_GiveDashGizmos_Equipment : ThingComp
    {
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            base.Notify_Unequipped(pawn);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }
}


By following these guidelines and utilizing GitHub Copilot, developers can efficiently create and maintain the ExGT WorldWar (Continued) mod, enhancing the RimWorld experience with innovative World War I-themed content.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

