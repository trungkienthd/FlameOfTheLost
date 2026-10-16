using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost.Editor
{
    public static class MapBlueprintGenerator
    {
        private const int ReferenceWidth = 200;
        private const int ReferenceHeight = 160;

        private static readonly (int x, int y)[] Directions =
        {
            (1, 0),
            (-1, 0),
            (0, 1),
            (0, -1)
        };

        public static MapBlueprint Generate(MapBlueprintSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            MapBlueprintGenerator.ValidateSettings(settings);

            var random = new System.Random(settings.Seed);

            var blueprint = new MapBlueprint
            {
                MapName = settings.MapName,
                Seed = settings.Seed,
                Width = settings.Width,
                Height = settings.Height,
                CellSize = settings.CellSize
            };

            MapBlueprintGenerator.GenerateRegions(blueprint, settings, random);
            MapBlueprintGenerator.GenerateConnections(blueprint);

            var cells = MapBlueprintGenerator.CreateCellGrid(blueprint);

            MapBlueprintGenerator.AssignRegions(blueprint, cells);

            MapBlueprintGenerator.GenerateRoads(blueprint, settings, cells);
            MapBlueprintGenerator.GenerateTerrain(blueprint, settings, cells);
            MapBlueprintGenerator.GeneratePlacements(blueprint, settings, cells, random);

            MapBlueprintGenerator.ValidateBlueprint(blueprint, cells);

            blueprint.Cells = new List<MapCellData>(blueprint.Width * blueprint.Height);

            for (int y = 0; y < blueprint.Height; y++)
            {
                for (int x = 0; x < blueprint.Width; x++)
                {
                    blueprint.Cells.Add(cells[x, y]);
                }
            }

            return blueprint;
        }

        private static void ValidateSettings(MapBlueprintSettings settings)
        {
            if (settings.Width < 120)
            {
                throw new ArgumentException("Map width must be at least 120 cells.");
            }
            if (settings.Height < 110)
            {
                throw new ArgumentException("Map height must be at least 110 cells.");
            }
            if (settings.CellSize <= 0f)
            {
                throw new ArgumentException("Cell size must be greater than zero.");
            }
            if (settings.RoadWidth < 1)
            {
                throw new ArgumentException("Road width must be at least one cell.");
            }
        }

        #region Region Generation
        private static void GenerateRegions(MapBlueprint blueprint, MapBlueprintSettings settings, System.Random random)
        {
            MapBlueprintGenerator.AddRegion
            (
                blueprint, settings, random,
                "R01", "Southern Gate", "Entrance",
                100, 12, 18, 12,
                true, true,
                "Transition from Greenvale Border Path."
            );
            MapBlueprintGenerator.AddRegion
            (
                blueprint, settings, random,
                "R02", "Southern Road", "Transition",
                100, 36, 22, 18,
                true, false,
                "Safe introductory road."
            );
            MapBlueprintGenerator.AddRegion
            (
                blueprint, settings, random,
                "R03", "Traveller's Camp", "SocialHub",
                100, 59, 34, 22,
                true, true,
                "NPCs, shops, quests, checkpoint and crafting."
            );
            MapBlueprintGenerator.AddRegion
            (
                blueprint, settings, random,
                "R04", "Central Crossroads", "Navigation",
                100, 88, 30, 24,
                true, true,
                "Main four-way road junction."
            );
            MapBlueprintGenerator.AddRegion
            (
                blueprint, settings, random,
                "R05", "Old Quarry", "Gathering",
                36, 67, 32, 24,
                false, false,
                "Stone, ore and resource gathering."
            );
            MapBlueprintGenerator.AddRegion
            (
                blueprint, settings, random,
                "R06", "Forest Trail", "Combat",
                100, 120, 24, 18,
                false, false,
                "Hostile woodland trail."
            );
            MapBlueprintGenerator.AddRegion
            (
                blueprint, settings, random,
                "R07", "Northern Woods", "Exploration",
                110, 145, 46, 20,
                false, false,
                "Dense forest and hidden resources."
            );
            MapBlueprintGenerator.AddRegion
            (
                blueprint, settings, random,
                "R08", "Western Ruins", "OptionalChallenge",
                42, 113, 34, 25,
                false, true,
                "Abandoned outpost and optional combat."
            );
            MapBlueprintGenerator.AddRegion
            (
                blueprint, settings, random,
                "R09", "Eastern Approach", "MainProgression",
                152, 89, 25, 23,
                true, false,
                "Road leading to the boss."
            );
            MapBlueprintGenerator.AddRegion
            (
                blueprint, settings, random,
                "R10", "Boss Arena", "Boss",
                172, 129, 36, 31,
                true, true,
                "Major boss encounter and eastern exit."
            );
        }
        private static void AddRegion(MapBlueprint blueprint, MapBlueprintSettings settings, System.Random random, string id, string name, string category, int referenceX, int referenceY, int referenceWidth, int referenceHeight, bool mandatory, bool handcrafted, string description)
        {
            float scaleX = settings.Width / (float)ReferenceWidth;
            float scaleY = settings.Height / (float)ReferenceHeight;

            int width   = Mathf.Max(8, Mathf.RoundToInt(referenceWidth * scaleX));
            int height  = Mathf.Max(8, Mathf.RoundToInt(referenceHeight * scaleY));

            int variation = settings.RegionPositionVariation;

            int offsetX = random.Next(-variation, variation + 1);
            int offsetY = random.Next(-variation, variation + 1);

            int centerX = Mathf.RoundToInt(referenceX * scaleX) + offsetX;
            int centerY = Mathf.RoundToInt(referenceY * scaleY) + offsetY;

            // Keep the entire region inside the map.
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            centerX = Mathf.Clamp(centerX, halfWidth, settings.Width - width + halfWidth);
            centerY = Mathf.Clamp(centerY, halfHeight, settings.Height - height + halfHeight);

            var region = new MapRegionData
            {
                Id = id,
                Name = name,
                Category = category,
                CenterX = centerX,
                CenterY = centerY,
                Width = width,
                Height = height,
                IsMandatory = mandatory,
                IsHandcrafted = handcrafted,
                Description = description
            };

            foreach (var existing in blueprint.Regions)
            {
                if (RegionsOverlap(region, existing))
                {
                    throw new InvalidOperationException($"Region {id} overlaps {existing.Id}. Reduce positional variation or adjust map dimensions.");
                }
            }

            blueprint.Regions.Add(region);
        }

        private static bool RegionsOverlap(MapRegionData a, MapRegionData b)
        {
            return a.MinX <= b.MaxX && a.MaxX >= b.MinX && a.MinY <= b.MaxY && a.MaxY >= b.MinY;
        }
        #endregion

        #region Graph Connections
        private static void GenerateConnections(MapBlueprint blueprint)
        {
            MapBlueprintGenerator.AddConnection(blueprint, "R01", "R02");
            MapBlueprintGenerator.AddConnection(blueprint, "R02", "R03");
            MapBlueprintGenerator.AddConnection(blueprint, "R03", "R04");

            MapBlueprintGenerator.AddConnection(blueprint, "R04", "R06");
            MapBlueprintGenerator.AddConnection(blueprint, "R06", "R07");

            MapBlueprintGenerator.AddConnection(blueprint, "R04", "R08");
            MapBlueprintGenerator.AddConnection(blueprint, "R08", "R05");

            MapBlueprintGenerator.AddConnection(blueprint, "R04", "R09");
            MapBlueprintGenerator.AddConnection(blueprint, "R09", "R10");

            MapBlueprintGenerator.AddConnection(blueprint, "R07", "R08", true, "ExplorationShortcut");

            MapBlueprintGenerator.AddConnection(blueprint, "R05", "R03", true, "ExplorationShortcut");
        }
        private static void AddConnection(MapBlueprint blueprint, string from, string to, bool optional = false, string type = "MainRoad")
        {
            var connectionData = new MapConnectionData
            {
                FromRegionId = from,
                ToRegionId = to,
                IsOptional = optional,
                ConnectionType = type
            };
            blueprint.Connections.Add(connectionData);
        }
        #endregion

        #region Cell Grid
        private static MapCellData[,] CreateCellGrid(MapBlueprint blueprint)
        {
            var cells = new MapCellData[blueprint.Width, blueprint.Height];

            for (int y = 0; y < blueprint.Height; y++)
            {
                for (int x = 0; x < blueprint.Width; x++)
                {
                    cells[x, y] = new MapCellData
                    {
                        X = x,
                        Y = y,
                        RegionId = "",
                        TerrainType = "Grass",
                        IsRoad = false
                    };
                }
            }

            return cells;
        }
        private static void AssignRegions(MapBlueprint blueprint, MapCellData[,] cells)
        {
            foreach (var region in blueprint.Regions)
            {
                for (int y = region.MinY; y <= region.MaxY; y++)
                {
                    for (int x = region.MinX; x <= region.MaxX; x++)
                    {
                        cells[x, y].RegionId = region.Id;
                    }
                }
            }
        }
        #endregion

        #region Road Generation
        private static void GenerateRoads(MapBlueprint blueprint, MapBlueprintSettings settings, MapCellData[,] cells)
        {
            foreach (var connection in blueprint.Connections)
            {
                var from    = MapBlueprintGenerator.GetRegion(blueprint, connection.FromRegionId);
                var to      = MapBlueprintGenerator.GetRegion(blueprint, connection.ToRegionId);

                var path = MapBlueprintGenerator.FindPath(blueprint, cells, from, to);
                if (path == null)
                {
                    blueprint.ValidationWarnings.Add($"Failed to generate road: {from.Id} -> {to.Id}");
                    continue;
                }

                connection.Path = path;

                foreach (var point in path)
                {
                    MapBlueprintGenerator.PaintRoad(blueprint, cells, point.X, point.Y, settings.RoadWidth);
                }
            }
        }
        private static List<MapCoordinateData> FindPath(MapBlueprint blueprint, MapCellData[,] cells, MapRegionData from, MapRegionData to)
        {
            int width = blueprint.Width;
            int height = blueprint.Height;

            int start = from.CenterY * width + from.CenterX;
            int goal = to.CenterY * width + to.CenterX;

            int total = width * height;

            var gScore = new int[total];
            var previous = new int[total];
            var closed = new bool[total];

            for (int i = 0; i < total; i++)
            {
                gScore[i] = int.MaxValue;
                previous[i] = -1;
            }

            var open = new List<int>();

            gScore[start] = 0;
            open.Add(start);

            while (open.Count > 0)
            {
                int bestIndex = 0;
                int bestScore = int.MaxValue;

                for (int i = 0; i < open.Count; i++)
                {
                    int node = open[i];

                    int x = node % width;
                    int y = node / width;

                    int heuristic = (Mathf.Abs(x - to.CenterX) + Mathf.Abs(y - to.CenterY)) * 10;

                    int score = gScore[node] + heuristic;

                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestIndex = i;
                    }
                }

                int current = open[bestIndex];
                open.RemoveAt(bestIndex);

                if (current == goal)
                {
                    return ReconstructPath(previous, current, width);
                }

                if (closed[current]) continue;

                closed[current] = true;

                int currentX = current % width;
                int currentY = current / width;

                foreach (var direction in Directions)
                {
                    int nextX = currentX + direction.x;
                    int nextY = currentY + direction.y;

                    if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height) continue;

                    int next = nextY * width + nextX;

                    if (closed[next]) continue;

                    var cell = cells[nextX, nextY];

                    int movementCost = 10;

                    // Strongly discourage roads crossing unrelated regions.
                    if (!string.IsNullOrEmpty(cell.RegionId) && cell.RegionId != from.Id && cell.RegionId != to.Id)
                    {
                        movementCost += 1000;
                    }

                    // Reuse existing roads when reasonable.
                    if (cell.IsRoad)
                    {
                        movementCost = Mathf.Max(1, movementCost - 4);
                    }

                    int tentative = gScore[current] + movementCost;

                    if (tentative >= gScore[next]) continue;

                    previous[next] = current;
                    gScore[next] = tentative;

                    if (!open.Contains(next))
                    {
                        open.Add(next);
                    }
                }
            }

            return null;
        }
        private static List<MapCoordinateData> ReconstructPath(int[] previous, int current, int width)
        {
            var result = new List<MapCoordinateData>();

            while (current >= 0)
            {
                var coordinateData = new MapCoordinateData(current % width, current / width);
                result.Add(coordinateData);

                current = previous[current];
            }

            result.Reverse();

            return result;
        }
        private static void PaintRoad(MapBlueprint blueprint, MapCellData[,] cells, int centerX, int centerY, int roadWidth)
        {
            int lower = (roadWidth - 1) / 2;
            int upper = roadWidth / 2;

            for (int offsetY = -lower; offsetY <= upper; offsetY++)
            {
                for (int offsetX = -lower; offsetX <= upper; offsetX++)
                {
                    int x = centerX + offsetX;
                    int y = centerY + offsetY;

                    if (x < 0 || x >= blueprint.Width || y < 0 || y >= blueprint.Height) continue;

                    cells[x, y].IsRoad = true;
                    cells[x, y].TerrainType = "Road";
                }
            }
        }
        #endregion

        #region Terrain Generation
        private static void GenerateTerrain(MapBlueprint blueprint, MapBlueprintSettings settings, MapCellData[,] cells)
        {
            float seedOffsetX = (int)((uint)settings.Seed % 100000u) + 1000f;
            float seedOffsetY = (int)((uint)(settings.Seed * 397L) % 100000u) + 2000f;

            for (int y = 0; y < blueprint.Height; y++)
            {
                for (int x = 0; x < blueprint.Width; x++)
                {
                    var cell = cells[x, y];

                    if (cell.IsRoad) continue;

                    float noise = Mathf.PerlinNoise(x * 0.045f + seedOffsetX, y * 0.045f + seedOffsetY);

                    cell.TerrainType = MapBlueprintGenerator.ClassifyTerrain(cell.RegionId, noise);
                }
            }
        }
        private static string ClassifyTerrain(string regionId, float noise)
        {
            switch (regionId)
            {
                case "R03":
                case "R04":
                    return noise > 0.65f ? "Grass" : "Dirt";

                case "R05":
                    return noise > 0.35f
                        ? "RockyGround"
                        : "Grass";

                case "R07":
                    return noise > 0.40f
                        ? "Forest"
                        : "Grass";

                case "R08":
                    return noise > 0.55f
                        ? "RockyGround"
                        : "Grass";

                case "R10":
                    return noise > 0.60f
                        ? "RockyGround"
                        : "Dirt";

                default:
                    if (noise > 0.68f) return "Forest";
                    if (noise > 0.48f) return "Grass";

                    return "Dirt";
            }
        }
        #endregion

        #region Object Placement
        private static void GeneratePlacements(MapBlueprint blueprint, MapBlueprintSettings settings, MapCellData[,] cells, System.Random random)
        {
            MapBlueprintGenerator.AddRequiredPlacement(blueprint, "PlayerEntry",            "Player Spawn",                 "PlayerSpawn",      "R01",  0,  0);
            MapBlueprintGenerator.AddRequiredPlacement(blueprint, "CampCheckpoint",         "Traveller's Camp Checkpoint",  "Checkpoint",       "R03", -5, -4);
            MapBlueprintGenerator.AddRequiredPlacement(blueprint, "CampMerchant",           "Merchant NPC",                 "NPC",              "R03", -6,  4);
            MapBlueprintGenerator.AddRequiredPlacement(blueprint, "CampRowan",              "Rowan NPC",                    "NPC",              "R03",  6,  4);
            MapBlueprintGenerator.AddRequiredPlacement(blueprint, "CampCraftingStation",    "Crafting Station",             "Crafting",         "R03",  5, -5);
            MapBlueprintGenerator.AddRequiredPlacement(blueprint, "BossSpawn",              "Main Boss",                    "Boss",             "R10",  0,  4);
            MapBlueprintGenerator.AddRequiredPlacement(blueprint, "BossAllySpawn",          "Temporary Ally Spawn",         "AllySpawn",        "R10", -6, -5);
            MapBlueprintGenerator.AddRequiredPlacement(blueprint, "EasternExit",            "Eastern Map Transition",       "MapTransition",    "R10",  0, 12);

            if (!settings.GenerateEnvironmentPlacements) return;

            int placementIndex = 0;

            foreach (var region in blueprint.Regions)
            {
                if (region.IsHandcrafted) continue;

                int targetCount = Mathf.RoundToInt(region.Area * settings.EnvironmentDensity * 0.035f);

                int attempts = targetCount * 20;

                while (targetCount > 0 && attempts-- > 0)
                {
                    int x = random.Next(region.MinX, region.MaxX + 1);
                    int y = random.Next(region.MinY, region.MaxY + 1);

                    if (!MapBlueprintGenerator.IsValidEnvironmentPosition(blueprint, cells, x, y)) continue;

                    string category = MapBlueprintGenerator.SelectEnvironmentCategory(region.Id, random);

                    var objectPlacementData = new MapObjectPlacementData
                    {
                        Id = $"ENV_{++placementIndex:0000}",
                        Name = category,
                        Category = category,
                        RegionId = region.Id,
                        X = x,
                        Y = y,
                        IsRequired = false,
                        Notes = "Suggested procedural placement."
                    };
                    blueprint.Placements.Add(objectPlacementData);

                    targetCount--;
                }
            }
        }
        private static void AddRequiredPlacement(MapBlueprint blueprint, string id, string name, string category, string regionId, int offsetX, int offsetY)
        {
            var region = MapBlueprintGenerator.GetRegion(blueprint, regionId);

            var objectPlacementData = new MapObjectPlacementData
            {
                Id = id,
                Name = name,
                Category = category,
                RegionId = regionId,

                X = Mathf.Clamp(region.CenterX + offsetX, region.MinX, region.MaxX),
                Y = Mathf.Clamp(region.CenterY + offsetY, region.MinY, region.MaxY),

                IsRequired = true,
                Notes = "Reserved gameplay placement."
            };
            blueprint.Placements.Add(objectPlacementData);
        }
        private static bool IsValidEnvironmentPosition(MapBlueprint blueprint, MapCellData[,] cells, int x, int y)
        {
            if (cells[x, y].IsRoad) return false;

            foreach (var placement in blueprint.Placements)
            {
                int dx = Mathf.Abs(placement.X - x);
                int dy = Mathf.Abs(placement.Y - y);

                if (dx <= 3 && dy <= 3) return false;
            }

            return true;
        }
        private static string SelectEnvironmentCategory(string regionId, System.Random random)
        {
            switch (regionId)
            {
                case "R05":
                    return random.NextDouble() < 0.75
                        ? "Stone"
                        : "OreNode";

                case "R07":
                    return random.NextDouble() < 0.80
                        ? "Tree"
                        : "Scenery";

                case "R06":
                    return random.NextDouble() < 0.60
                        ? "Tree"
                        : "Scenery";

                default:
                    return random.NextDouble() < 0.50
                        ? "Tree"
                        : "Scenery";
            }
        }
        #endregion

        #region Validation
        private static void ValidateBlueprint(MapBlueprint blueprint, MapCellData[,] cells)
        {
            foreach (var connection in blueprint.Connections)
            {
                if (connection.Path == null || connection.Path.Count == 0)
                {
                    blueprint.ValidationWarnings.Add($"Missing connection: {connection.FromRegionId} -> {connection.ToRegionId}");
                }
            }

            foreach (var placement in blueprint.Placements)
            {
                if (placement.X < 0 || placement.X >= blueprint.Width || placement.Y < 0 || placement.Y >= blueprint.Height)
                {
                    blueprint.ValidationWarnings.Add($"Placement outside map: {placement.Id}");
                }
            }

            MapBlueprintGenerator.ValidateGraphConnectivity(blueprint);
        }
        private static void ValidateGraphConnectivity(MapBlueprint blueprint)
        {
            var visited = new HashSet<string>();
            var queue = new Queue<string>();

            queue.Enqueue("R01");
            visited.Add("R01");

            while (queue.Count > 0)
            {
                string current = queue.Dequeue();

                foreach (var connection in blueprint.Connections)
                {
                    if (connection.Path == null || connection.Path.Count == 0) continue;

                    string neighbour = null;

                    if (connection.FromRegionId == current)
                        neighbour = connection.ToRegionId;
                    else if (connection.ToRegionId == current)
                        neighbour = connection.FromRegionId;

                    if (neighbour == null || !visited.Add(neighbour)) continue;

                    queue.Enqueue(neighbour);
                }
            }

            foreach (var region in blueprint.Regions)
            {
                if (!visited.Contains(region.Id))
                {
                    blueprint.ValidationWarnings.Add($"Unreachable region: {region.Id}");
                }
            }
        }
        #endregion

        private static MapRegionData GetRegion(MapBlueprint blueprint, string regionId)
        {
            foreach (var region in blueprint.Regions)
            {
                if (region.Id == regionId) return region;
            }

            throw new InvalidOperationException($"Unknown region: {regionId}");
        }
    }
}
