using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
	public abstract class Manager<T>
	{
		public static List<Policy> policies = new List<Policy>();
		public static List<MapActivePolicy> activePolicies = new List<MapActivePolicy>();
		public static List<T> links = new List<T>();
		public static bool showPaste = false;
        public static Dictionary<WorkTypeDef, List<WorkGiverDef>> workgivers = new Dictionary<WorkTypeDef, List<WorkGiverDef>>();
        
		static Manager()
        {
            Policy defaultPolicy = new Policy(policies.Count, "BPC.Auto".Translate());
            policies.Add(defaultPolicy);
            activePolicies.Add(new MapActivePolicy(0, defaultPolicy));
        }

        public static void ForceInit()
        {
            policies = new List<Policy>();
            activePolicies = new List<MapActivePolicy>();
            links = new List<T>();
            Policy defaultPolicy = new Policy(policies.Count, "BPC.Auto".Translate());
            policies.Add(defaultPolicy);
            activePolicies.Add(new MapActivePolicy(0, defaultPolicy));
        }

		public static IEnumerable<Pawn> Colonists()
		{
			try
			{
                return from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer) where p.IsColonist select p;
			}
			catch (Exception) 
			{ 
				return new List<Pawn>(); 
			}
        }

        public static List<WorkGiverDef> GetWorkGivers(WorkTypeDef workType)
        {
            if (workgivers.TryGetValue(workType, out var result))
                return result;

            var list = DefDatabase<WorkGiverDef>.AllDefsListForReading
                .Where(x => x.workType == workType)
                .ToList();
            workgivers.Add(workType, list);
            
			return list;
        }

        private static bool _dirtyPolicy = false;
		public static bool DirtyPolicy
		{
			get
			{
				return _dirtyPolicy;
			}

			set
			{
				_dirtyPolicy = value;
			}
		}

		public static Policy GetActivePolicy()
		{
			return GetActivePolicy(Find.CurrentMap.uniqueID);
		}

		public static void SetActivePolicy(Policy policy)
		{
			SetActivePolicy(Find.CurrentMap.uniqueID, policy);
		}

		public static Policy GetActivePolicy(int mapId)
		{
			if (activePolicies == null)
			{
				Manager<T>.ForceInit();
			}

			MapActivePolicy mapPolicy = activePolicies.Find(x => x.mapId == mapId);
			if (mapPolicy == null)
			{
				//new map! create default
				mapPolicy = new MapActivePolicy(mapId, policies[0]);
				activePolicies.Add(mapPolicy);
			}
			return mapPolicy.activePolicy;
		}

		public static Policy GetPolicy(int selected)
		{
			return policies.Find(x => x.id == selected);
		}

        public static MapActivePolicy GetActiveMap(int mapId)
        {
            if (activePolicies == null)
            {
				//create default
				GetActivePolicy(mapId);
            }

			return activePolicies.Find(x => x.mapId == mapId);
        }

        public static void SetActivePolicy(int mapId, Policy policy)
		{
			MapActivePolicy map = activePolicies.Find(x => x.mapId == mapId);
			if (map != null)
			{
				map.activePolicy = policy;
			}
			else
			{
				activePolicies.Add(new MapActivePolicy(mapId, policy));
			}
		}

		public static void MoveLinksToMap(int srcMapId, int dstMapId)
		{
			if (srcMapId == -1)
			{
				//this means there is not last map and nothing should be done.
				Log.Warning("[BPC] Couldn't copy settings to new map since last map does not exit");
				return;
			}

			foreach (T link in links)
			{
				if (link.GetType() == typeof(WorkLink))
				{
					if (link.ChangeType<WorkLink>().mapId == srcMapId)
					{
                        link.ChangeType<WorkLink>().mapId = dstMapId;
                    }
				}

				if (link.GetType() == typeof(ScheduleLink))
				{
                    if (link.ChangeType<ScheduleLink>().mapId == srcMapId)
                    {
                        link.ChangeType<ScheduleLink>().mapId = dstMapId;
                    }
                }

				if (link.GetType() == typeof(AssignLink))
				{
                    if (link.ChangeType<AssignLink>().mapId == srcMapId)
                    {
                        link.ChangeType<AssignLink>().mapId = dstMapId;
                    }
                }

				if (link.GetType() == typeof(AnimalLink))
				{
                    if (link.ChangeType<AnimalLink>().mapId == srcMapId)
                    {
                        link.ChangeType<AnimalLink>().mapId = dstMapId;
                    }
                }

				if (link.GetType() == typeof(MechLink))
				{
                    if (link.ChangeType<MechLink>().mapId == srcMapId)
                    {
                        link.ChangeType<MechLink>().mapId = dstMapId;
                    }
                }

                if (link.GetType() == typeof(RobotLink))
                {
                    if (link.ChangeType<RobotLink>().mapId == srcMapId)
                    {
                        link.ChangeType<RobotLink>().mapId = dstMapId;
                    }
                }
                
				if (link.GetType() == typeof(WeaponsLink))
                {
                    if (link.ChangeType<WeaponsLink>().mapId == srcMapId)
                    {
                        link.ChangeType<WeaponsLink>().mapId = dstMapId;
                    }
                }
            }
		}

		public static bool FoodPolicyExists(FoodPolicy foodPolicy)
		{
			foreach (FoodPolicy food in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
			{
				if (food.Equals(foodPolicy))
				{
					return true;
				}
			}
			return false;
		}

		public static FoodPolicy _defaultFoodPolicy = null;
		public static FoodPolicy DefaultFoodPolicy
		{
			get
			{
				if (_defaultFoodPolicy == null)
				{
					_defaultFoodPolicy = Current.Game.foodRestrictionDatabase.DefaultFoodRestriction();
				}
				return _defaultFoodPolicy;
			}

			set
			{
				_defaultFoodPolicy = value;
			}
		}

		public static ReadingPolicy _defaultReadingPolicy = null;
		public static ReadingPolicy DefaultReadingPolicy
		{
			get
			{
				if (_defaultReadingPolicy == null)
				{
					_defaultReadingPolicy = Current.Game.readingPolicyDatabase.DefaultReadingPolicy();
				}
				return _defaultReadingPolicy;
			}

			set
			{
				_defaultReadingPolicy = value;
			}
		}

        public static MedicalCareCategory DefaultMedsPolicy
        {
            get
            {
                return Current.Game.playSettings.defaultCareForColonist;
            }

            set
            {
                Current.Game.playSettings.defaultCareForColonist = value;
            }
        }
    }
}

