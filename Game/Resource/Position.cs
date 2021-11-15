﻿using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PacificEngine.OW_CommonResources.Game.Resource
{
    public static class Position
    {
        private static float lastUpdate = 0f;
        public static bool debugMode { get; set; } = false;

        private static ScreenPrompt parentPrompt = new ScreenPrompt("");
        private static ScreenPrompt positionPrompt = new ScreenPrompt("");
        private static ScreenPrompt velocityPrompt = new ScreenPrompt("");

        private delegate OWRigidbody body();
        public delegate Vector3 vector();

        public enum HeavenlyBodies
        {
            Sun,
            SunStation,
            AshTwin,
            EmberTwin,
            TimberHearth,
            TimberHearthProbe,
            Attlerock,
            BrittleHollow,
            HollowLantern,
            GiantsDeep,
            ProbeCannon,
            NomaiProbe,
            DarkBramble,
            InnerDarkBramble_Hub,
            InnerDarkBramble_EscapePod,
            InnerDarkBramble_Nest,
            InnerDarkBramble_Feldspar,
            InnerDarkBramble_Gutter,
            InnerDarkBramble_Vessel,
            InnerDarkBramble_Maze,
            InnerDarkBramble_SmallNest,
            Interloper,
            WhiteHole,
            WhiteHoleStation,
            Stranger,
            DreamWorld,
            QuantumMoon,
            EyeOfTheUniverse,
            EyeOfTheUniverse_Vessel
        }

        private static Dictionary<HeavenlyBodies, body> bodyLookup = new Dictionary<HeavenlyBodies, body>();
        private static Dictionary<HeavenlyBodies, OWRigidbody> bodies = new Dictionary<HeavenlyBodies, OWRigidbody>();

        private static OWRigidbody getPlayerBody()
        {
            if (PlayerState.IsInsideShip())
            {
                return Locator.GetShipBody();
            }
            else
            {
                return Locator.GetPlayerBody();
            }
        }

        public static void Start()
        {
            bodies.Clear();
            bodyLookup.Clear();
            bodyLookup.Add(HeavenlyBodies.Sun, () => Locator.GetAstroObject(AstroObject.Name.Sun)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.SunStation, () => Locator.GetWarpReceiver(NomaiWarpPlatform.Frequency.SunStation)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.AshTwin, () => Locator.GetAstroObject(AstroObject.Name.TowerTwin)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.EmberTwin, () => Locator.GetAstroObject(AstroObject.Name.CaveTwin)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.TimberHearth, () => Locator.GetAstroObject(AstroObject.Name.TimberHearth)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.TimberHearthProbe, () => Locator.GetAstroObject(AstroObject.Name.TimberHearth)?.GetSatellite()?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.Attlerock, () => Locator.GetAstroObject(AstroObject.Name.TimberHearth)?.GetMoon()?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.BrittleHollow, () => Locator.GetAstroObject(AstroObject.Name.BrittleHollow)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.HollowLantern, () => Locator.GetAstroObject(AstroObject.Name.BrittleHollow)?.GetMoon()?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.GiantsDeep, () => Locator.GetAstroObject(AstroObject.Name.GiantsDeep)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.ProbeCannon, () => Locator.GetAstroObject(AstroObject.Name.ProbeCannon)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.NomaiProbe, () => Locator.GetAstroObject(AstroObject.Name.ProbeCannon)?.GetComponent<OrbitalProbeLaunchController>()?.GetValue<OWRigidbody>("_probeBody"));
            bodyLookup.Add(HeavenlyBodies.DarkBramble, () => Locator.GetAstroObject(AstroObject.Name.DarkBramble)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.InnerDarkBramble_Hub, () => Helper.getSector(Sector.Name.BrambleDimension).Find(body => OuterFogWarpVolume.Name.Hub.Equals(body?.GetComponentInChildren<OuterFogWarpVolume>()?.GetName()))?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.InnerDarkBramble_EscapePod, () => Helper.getSector(Sector.Name.BrambleDimension).Find(body => OuterFogWarpVolume.Name.EscapePod.Equals(body?.GetComponentInChildren<OuterFogWarpVolume>()?.GetName()))?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.InnerDarkBramble_Nest, () => Helper.getSector(Sector.Name.BrambleDimension).Find(body => OuterFogWarpVolume.Name.AnglerNest.Equals(body?.GetComponentInChildren<OuterFogWarpVolume>()?.GetName()))?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.InnerDarkBramble_Feldspar, () => Helper.getSector(Sector.Name.BrambleDimension).Find(body => OuterFogWarpVolume.Name.Pioneer.Equals(body?.GetComponentInChildren<OuterFogWarpVolume>()?.GetName()))?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.InnerDarkBramble_Gutter, () => Helper.getSector(Sector.Name.BrambleDimension).Find(body => OuterFogWarpVolume.Name.ExitOnly.Equals(body?.GetComponentInChildren<OuterFogWarpVolume>()?.GetName()))?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.InnerDarkBramble_Vessel, () => Helper.getSector(Sector.Name.VesselDimension).Find(body => OuterFogWarpVolume.Name.Vessel.Equals(body?.GetComponentInChildren<OuterFogWarpVolume>()?.GetName()))?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.InnerDarkBramble_Maze, () => Helper.getSector(Sector.Name.BrambleDimension).Find(body => OuterFogWarpVolume.Name.Cluster.Equals(body?.GetComponentInChildren<OuterFogWarpVolume>()?.GetName()))?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.InnerDarkBramble_SmallNest, () => Helper.getSector(Sector.Name.BrambleDimension).Find(body => OuterFogWarpVolume.Name.SmallNest.Equals(body?.GetComponentInChildren<OuterFogWarpVolume>()?.GetName()))?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.Interloper, () => Locator.GetAstroObject(AstroObject.Name.Comet)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.WhiteHole, () => Locator.GetAstroObject(AstroObject.Name.WhiteHole)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.WhiteHoleStation, () => Locator.GetAstroObject(AstroObject.Name.WhiteHoleTarget)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.Stranger, () => Locator.GetAstroObject(AstroObject.Name.RingWorld)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.DreamWorld, () => Locator.GetAstroObject(AstroObject.Name.DreamWorld)?.GetAttachedOWRigidbody());
            bodyLookup.Add(HeavenlyBodies.QuantumMoon, () => Locator.GetAstroObject(AstroObject.Name.QuantumMoon)?.GetAttachedOWRigidbody());
        }

        public static void Awake()
        {
            bodies.Clear();
        }

        public static void Destroy()
        {
        }

        public static void Update()
        {
            if (debugMode && Locator.GetPlayerBody())
            {
                if (Locator.GetPromptManager()?.GetScreenPromptList(PromptPosition.LowerLeft)?.Contains(parentPrompt) == false)
                {
                    Locator.GetPromptManager().AddScreenPrompt(parentPrompt, PromptPosition.LowerLeft, true);
                    Locator.GetPromptManager().AddScreenPrompt(positionPrompt, PromptPosition.LowerLeft, true);
                    Locator.GetPromptManager().AddScreenPrompt(velocityPrompt, PromptPosition.LowerLeft, true);
                }

                if (Time.time - lastUpdate > 0.2f)
                {
                    lastUpdate = Time.time;
                    var list = Position.getClosest(getPlayerBody().GetPosition());
                    var item = list[0] == Position.HeavenlyBodies.TimberHearthProbe ? list[1] : list[0];
                    var parent = Position.getBody(item);
                    if (parent)
                    {
                        parentPrompt.SetText("Parent: " + item);
                        positionPrompt.SetText("Position: " + (parent.transform.InverseTransformPoint(getPlayerBody().GetPosition())));
                        velocityPrompt.SetText("Velocity: " + (getPlayerBody().GetVelocity() - parent.GetVelocity()));
                    }
                    parentPrompt?.SetVisibility(parent != null);
                    positionPrompt?.SetVisibility(parent != null);
                    velocityPrompt?.SetVisibility(parent != null);
                }
            }
            else
            {
                parentPrompt?.SetVisibility(false);
                positionPrompt?.SetVisibility(false);
                velocityPrompt?.SetVisibility(false);
            }
        }

        public static OWRigidbody getBody(HeavenlyBodies body)
        {
            OWRigidbody obj;
            if (!bodies.TryGetValue(body, out obj))
            {
                obj = bodyLookup[body].Invoke();
                bodies[body] = obj;
            }
            else if (obj.gameObject == null)
            {
                obj = bodyLookup[body].Invoke();
                bodies[body] = obj;
            }
            return obj.gameObject == null ? null : obj;
        }

        public static List<HeavenlyBodies> getClosest(Vector3 position)
        {
            var keys = new HeavenlyBodies[bodyLookup.Count];
            bodyLookup.Keys.CopyTo(keys, 0);
            return getClosest(position, keys);
        }

        public static List<HeavenlyBodies> getClosest(Vector3 position, params HeavenlyBodies[] include)
        {
            var distance = new Dictionary<HeavenlyBodies, float?>();
            var keys = new List<HeavenlyBodies>(include);
            keys.Sort((v1, v2) =>
            {
                float? distanceV1;
                float? distanceV2;
                if (!distance.TryGetValue(v1, out distanceV1))
                {
                    distanceV1 = getBody(v1)?.transform?.InverseTransformPoint(position).sqrMagnitude;
                    distance.Add(v1, distanceV1);
                }
                if (!distance.TryGetValue(v2, out distanceV2))
                {
                    distanceV2 = getBody(v2)?.transform?.InverseTransformPoint(position).sqrMagnitude;
                    distance.Add(v2, distanceV2);
                }
                if (distanceV1.HasValue && distanceV2.HasValue)
                {
                    return (int)(distanceV1.GetValueOrDefault(0f) - distanceV2.GetValueOrDefault(0f));
                }
                if (distanceV1.HasValue)
                {
                    return -1;
                }
                if (distanceV2.HasValue)
                {
                    return 1;
                }
                return 0;
            });
            return keys;
        }

    }
}
