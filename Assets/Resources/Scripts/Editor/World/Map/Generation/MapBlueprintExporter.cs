using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FlameOfTheLost.Editor
{
    public static class MapBlueprintExporter
    {
        public static void Export(MapBlueprint blueprint, string outputDirectory, int overviewScale = 2)
        {
            if (blueprint == null) throw new ArgumentNullException(nameof(blueprint));

            if (string.IsNullOrWhiteSpace(outputDirectory)) throw new ArgumentException("Invalid output directory.");

            Directory.CreateDirectory(outputDirectory);

            File.WriteAllText(Path.Combine(outputDirectory, "RegionMap.txt"),           MapBlueprintExporter.RenderAsciiMap(blueprint, true, overviewScale),    Encoding.UTF8);
            File.WriteAllText(Path.Combine(outputDirectory, "TerrainMap.txt"),          MapBlueprintExporter.RenderAsciiMap(blueprint, false, 1),               Encoding.UTF8);
            File.WriteAllText(Path.Combine(outputDirectory, "MapStatistics.md"),        MapBlueprintExporter.GenerateStatistics(blueprint),                     Encoding.UTF8);
            File.WriteAllText(Path.Combine(outputDirectory, "GenerationReport.md"),     MapBlueprintExporter.GenerateReport(blueprint),                         Encoding.UTF8);
            File.WriteAllText(Path.Combine(outputDirectory, "MapBlueprint.json"),       JsonUtility.ToJson(blueprint, true),                                    Encoding.UTF8);
            File.WriteAllText(Path.Combine(outputDirectory, "RegionLayout.csv"),        MapBlueprintExporter.GenerateRegionCsv(blueprint),                      Encoding.UTF8);
            File.WriteAllText(Path.Combine(outputDirectory, "ObjectPlacements.csv"),    MapBlueprintExporter.GeneratePlacementCsv(blueprint),                   Encoding.UTF8);
        }

        #region ASCII Rendering
        public static string RenderAsciiMap(MapBlueprint blueprint, bool regionMode, int scale = 1)
        {
            scale = Mathf.Max(1, scale);

            var cells = MapBlueprintExporter.BuildCellLookup(blueprint);

            int outputWidth = Mathf.CeilToInt(blueprint.Width / (float)scale);
            int outputHeight = Mathf.CeilToInt(blueprint.Height / (float)scale);

            var output = new char[outputWidth, outputHeight];

            for (int y = 0; y < outputHeight; y++)
            {
                for (int x = 0; x < outputWidth; x++)
                {
                    int sourceX = Mathf.Min(blueprint.Width - 1, x * scale + scale / 2);
                    int sourceY = Mathf.Min(blueprint.Height - 1, y * scale + scale / 2);

                    var cell = cells[sourceX, sourceY];

                    output[x, y] = regionMode
                        ? MapBlueprintExporter.GetRegionSymbol(cell)
                        : MapBlueprintExporter.GetTerrainSymbol(cell);
                }
            }

            // Overlay the actual road geometry.
            for (int y = 0; y < outputHeight; y++)
            {
                for (int x = 0; x < outputWidth; x++)
                {
                    if (MapBlueprintExporter.ContainsRoad(cells, blueprint, x * scale, y * scale, scale))
                    {
                        output[x, y] = '=';
                    }
                }
            }

            if (regionMode)
            {
                foreach (var region in blueprint.Regions)
                {
                    int x = region.CenterX / scale;
                    int y = region.CenterY / scale;

                    if (x >= 0 && x < outputWidth && y >= 0 && y < outputHeight)
                    {
                        output[x, y] = MapBlueprintExporter.GetRegionDigit(region.Id);
                    }
                }
            }
            else
            {
                foreach (var placement in blueprint.Placements)
                {
                    if (!placement.IsRequired) continue;

                    if (placement.X >= 0 && placement.X < outputWidth && placement.Y >= 0 && placement.Y < outputHeight)
                    {
                        output[placement.X, placement.Y] = MapBlueprintExporter.GetPlacementSymbol(placement.Category);
                    }
                }
            }

            var sb = new StringBuilder();

            sb.AppendLine($"MAP: {blueprint.MapName}");
            sb.AppendLine($"SEED: {blueprint.Seed}");
            sb.AppendLine($"DIMENSIONS: {blueprint.Width} x {blueprint.Height} cells");
            sb.AppendLine($"DISPLAY SCALE: 1 character = {scale} x {scale} cells");

            sb.AppendLine();
            sb.AppendLine("NORTH ↑");
            sb.AppendLine();

            sb.Append('+');
            sb.Append(new string('-', outputWidth));
            sb.AppendLine("+");

            for (int y = outputHeight - 1; y >= 0; y--)
            {
                sb.Append('|');

                for (int x = 0; x < outputWidth; x++)
                {
                    sb.Append(output[x, y]);
                }

                sb.AppendLine("|");
            }

            sb.Append('+');
            sb.Append(new string('-', outputWidth));
            sb.AppendLine("+");

            sb.AppendLine();
            sb.AppendLine("SOUTH ↓");
            sb.AppendLine();

            if (regionMode)
            {
                sb.AppendLine("REGION LEGEND");

                foreach (var region in blueprint.Regions)
                {
                    sb.AppendLine($"{MapBlueprintExporter.GetRegionDigit(region.Id)} = {region.Id}: {region.Name}");
                }

                sb.AppendLine("= = Road");
                sb.AppendLine(". = Outside named regions");
            }
            else
            {
                sb.AppendLine("TERRAIN LEGEND");
                sb.AppendLine(". = Grass");
                sb.AppendLine(", = Dirt");
                sb.AppendLine("F = Forest");
                sb.AppendLine("^ = Rocky Ground");
                sb.AppendLine("= = Road");
                sb.AppendLine("P = Player Spawn");
                sb.AppendLine("C = Checkpoint");
                sb.AppendLine("N = NPC");
                sb.AppendLine("W = Crafting Station");
                sb.AppendLine("B = Boss");
                sb.AppendLine("A = Ally Spawn");
                sb.AppendLine("E = Map Exit");
            }

            return sb.ToString();
        }
        private static MapCellData[,] BuildCellLookup(MapBlueprint blueprint)
        {
            var cells = new MapCellData[blueprint.Width, blueprint.Height];

            foreach (var cell in blueprint.Cells)
            {
                cells[cell.X, cell.Y] = cell;
            }

            return cells;
        }
        private static bool ContainsRoad(MapCellData[,] cells, MapBlueprint blueprint, int startX, int startY, int scale)
        {
            for (int y = startY; y < Mathf.Min(startY + scale, blueprint.Height); y++)
            {
                for (int x = startX; x < Mathf.Min(startX + scale, blueprint.Width); x++)
                {
                    if (cells[x, y].IsRoad) return true;
                }
            }

            return false;
        }
        private static char GetRegionSymbol(MapCellData cell)
        {
            if (cell.IsRoad) return '=';

            if (string.IsNullOrEmpty(cell.RegionId)) return '.';

            return MapBlueprintExporter.GetRegionDigit(cell.RegionId);
        }
        private static char GetRegionDigit(string regionId)
        {
            if (string.IsNullOrEmpty(regionId)) return '?';

            if (int.TryParse(regionId.Substring(1), out int number))
            {
                return (char)('0' + number % 10);
            }

            return '?';
        }
        private static char GetTerrainSymbol(MapCellData cell)
        {
            if (cell.IsRoad) return '=';

            switch (cell.TerrainType)
            {
                case "Grass":
                    return '.';

                case "Dirt":
                    return ',';

                case "Forest":
                    return 'F';

                case "RockyGround":
                    return '^';

                default:
                    return '?';
            }
        }
        private static char GetPlacementSymbol(string category)
        {
            switch (category)
            {
                case "PlayerSpawn":
                    return 'P';

                case "Checkpoint":
                    return 'C';

                case "NPC":
                    return 'N';

                case "Crafting":
                    return 'W';

                case "Boss":
                    return 'B';

                case "AllySpawn":
                    return 'A';

                case "MapTransition":
                    return 'E';

                default:
                    return '*';
            }
        }
        #endregion

        #region Statistics
        public static string GenerateStatistics(MapBlueprint blueprint)
        {
            var sb = new StringBuilder();

            sb.AppendLine("# Map Statistics");
            sb.AppendLine();

            sb.AppendLine($"**Map:** {blueprint.MapName}  ");
            sb.AppendLine($"**Seed:** {blueprint.Seed}  ");
            sb.AppendLine();

            int totalCells = blueprint.Width * blueprint.Height;

            double worldWidth   = blueprint.Width   * blueprint.CellSize;
            double worldHeight  = blueprint.Height  * blueprint.CellSize;

            sb.AppendLine("## Overall Dimensions");
            sb.AppendLine();

            sb.AppendLine("| Property | Value |");
            sb.AppendLine("|---|---:|");

            sb.AppendLine($"| Width | {blueprint.Width} cells |");
            sb.AppendLine($"| Height | {blueprint.Height} cells |");

            sb.AppendLine($"| Total Area | {totalCells:N0} cells |");

            sb.AppendLine($"| Cell Size | {blueprint.CellSize} Unity units |");

            sb.AppendLine($"| World Width | {worldWidth:F2} units |");
            sb.AppendLine($"| World Height | {worldHeight:F2} units |");

            sb.AppendLine($"| World Area | {worldWidth * worldHeight:F2} square units |");

            sb.AppendLine($"| Regions | {blueprint.Regions.Count} |");

            sb.AppendLine($"| Connections | {blueprint.Connections.Count} |");

            sb.AppendLine($"| Suggested Object Placements | {blueprint.Placements.Count} |");

            sb.AppendLine();

            sb.AppendLine("## Region Statistics");
            sb.AppendLine();

            sb.AppendLine("| ID | Region | Centre | Bounds | Size | Area (Cells) |");

            sb.AppendLine("|---|---|---|---|---|---:|");

            foreach (var region in blueprint.Regions)
            {
                sb.AppendLine
                (
                    $"| {region.Id} " +
                    $"| {region.Name} " +
                    $"| ({region.CenterX}, {region.CenterY}) " +
                    $"| ({region.MinX}, {region.MinY})–" +
                    $"({region.MaxX}, {region.MaxY}) " +
                    $"| {region.Width} × {region.Height} " +
                    $"| {region.Area:N0} |"
                );
            }

            sb.AppendLine();

            sb.AppendLine("## Terrain Distribution");
            sb.AppendLine();

            sb.AppendLine("| Terrain | Cells | Percentage |");

            sb.AppendLine("|---|---:|---:|");

            var terrainGroups = blueprint.Cells
                .GroupBy(x => x.TerrainType)
                .OrderByDescending(x => x.Count());

            foreach (var group in terrainGroups)
            {
                int count = group.Count();

                float percentage = count * 100f / totalCells;

                sb.AppendLine($"| {group.Key} | {count:N0} | {percentage:F2}% |");
            }

            sb.AppendLine();

            sb.AppendLine("## Object Placement Statistics");
            sb.AppendLine();

            sb.AppendLine("| Category | Count |");
            sb.AppendLine("|---|---:|");

            var placementGroups = blueprint.Placements
                .GroupBy(x => x.Category)
                .OrderBy(x => x.Key);

            foreach (var group in placementGroups)
            {
                sb.AppendLine($"| {group.Key} | {group.Count()} |");
            }

            return sb.ToString();
        }
        #endregion

        #region Generation Report
        public static string GenerateReport(MapBlueprint blueprint)
        {
            var sb = new StringBuilder();

            sb.AppendLine("# Map Generation Report");
            sb.AppendLine();

            sb.AppendLine($"Map: {blueprint.MapName}  ");
            sb.AppendLine($"Seed: {blueprint.Seed}  ");

            sb.AppendLine();
            sb.AppendLine("## Region Connections");
            sb.AppendLine();

            foreach (var connection in blueprint.Connections)
            {
                string type = connection.IsOptional
                    ? "Optional"
                    : "Primary";

                sb.AppendLine($"- {connection.FromRegionId} ↔ {connection.ToRegionId} ({type}, {connection.Path.Count} path cells)");
            }

            sb.AppendLine();
            sb.AppendLine("## Validation");
            sb.AppendLine();

            if (blueprint.ValidationWarnings.Count == 0)
            {
                sb.AppendLine("No validation warnings were detected.");
            }
            else
            {
                foreach (string warning in blueprint.ValidationWarnings)
                {
                    sb.AppendLine($"- WARNING: {warning}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("## Notes");
            sb.AppendLine();

            sb.AppendLine("- This blueprint is intended for manual Unity scene construction.");
            sb.AppendLine("- Region dimensions represent rectangular design boundaries.");
            sb.AppendLine("- Procedural placements are suggestions, not final prefab instances.");
            sb.AppendLine("- Logical connectivity has been checked, but collider-aware traversal has not.");
            sb.AppendLine("- Important gameplay structures should be manually reviewed.");

            return sb.ToString();
        }
        #endregion

        #region CSV Export
        private static string GenerateRegionCsv(MapBlueprint blueprint)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Id,Name,Category,CenterX,CenterY,MinX,MinY,MaxX,MaxY,Width,Height,Area,Mandatory,Handcrafted");

            foreach (var region in blueprint.Regions)
            {
                sb.AppendLine
                (
                    $"{MapBlueprintExporter.Csv(region.Id)}," +
                    $"{MapBlueprintExporter.Csv(region.Name)}," +
                    $"{MapBlueprintExporter.Csv(region.Category)}," +
                    $"{region.CenterX}," +
                    $"{region.CenterY}," +
                    $"{region.MinX}," +
                    $"{region.MinY}," +
                    $"{region.MaxX}," +
                    $"{region.MaxY}," +
                    $"{region.Width}," +
                    $"{region.Height}," +
                    $"{region.Area}," +
                    $"{region.IsMandatory}," +
                    $"{region.IsHandcrafted}"
                );
            }

            return sb.ToString();
        }

        private static string GeneratePlacementCsv(MapBlueprint blueprint)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Id,Name,Category,RegionId,X,Y,Required,Notes");

            foreach (var placement in blueprint.Placements)
            {
                sb.AppendLine
                (
                    $"{MapBlueprintExporter.Csv(placement.Id)}," +
                    $"{MapBlueprintExporter.Csv(placement.Name)}," +
                    $"{MapBlueprintExporter.Csv(placement.Category)}," +
                    $"{MapBlueprintExporter.Csv(placement.RegionId)}," +
                    $"{placement.X}," +
                    $"{placement.Y}," +
                    $"{placement.IsRequired}," +
                    $"{MapBlueprintExporter.Csv(placement.Notes)}"
                );
            }

            return sb.ToString();
        }
        private static string Csv(string value)
        {
            value ??= "";

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        #endregion
    }
}
