using System;
using ContractConfigurator.Parameters;
using UnityEngine;

namespace PrincipiaContracts
{
	class PrincipiaContractParameter : VesselParameter
	{

		private string locationName;
		private double minDistance;
		private double maxDistance;

		public PrincipiaContractParameter():base(null)
		{

		}
		public PrincipiaContractParameter(string title):base(title)
		{

		}
		public PrincipiaContractParameter(string locationName, double minDistance, double maxDistance, string title = null):base(title)
		{
			this.locationName = locationName;
			this.minDistance = minDistance;
			this.maxDistance = maxDistance;

			CreateDelegates();
		}

		protected override string GetParameterTitle()
		{
			if (!string.IsNullOrEmpty(title)) {
				return title;
			}

			return "Stay between " + minDistance.ToString() + " and " + maxDistance.ToString() + " of " + locationName;

		}

		public PrincipiaContractParameter(string locationName, double minDistance, double maxDistance)
		{
			this.locationName = locationName;
			this.minDistance = minDistance;
			this.maxDistance = maxDistance;
		}

		protected override void OnParameterSave(ConfigNode node)
		{
			base.OnParameterSave(node);
			node.AddValue("locationName", locationName);
			node.AddValue("minDistance", minDistance);
			node.AddValue("maxDistance", maxDistance);
		}

		protected override void OnParameterLoad(ConfigNode node)
		{
			try {
				base.OnParameterLoad(node);
				locationName = node.GetValue("locationName");
				minDistance = Convert.ToDouble(node.GetValue("minDistance"));
				maxDistance = Convert.ToDouble(node.GetValue("maxDistance"));

				CreateDelegates();
			}
			finally {
				ParameterDelegate<Vessel>.OnDelegateContainerLoad(node);
			}
		}

		protected void CreateDelegates()
		{
			AddParameter(new ParameterDelegate<Vessel>(GetParameterTitle(), CheckDistances));
		}

		private float lastUpdate = 0.0f;
		private const float UPDATE_FREQUENCY = 0.25f;
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (Time.fixedTime - lastUpdate > UPDATE_FREQUENCY) {
				lastUpdate = Time.fixedTime;
				CheckVessel(FlightGlobals.ActiveVessel);
			}
		}

		/// <summary>
		/// Whether this vessel meets the parameter condition.
		/// </summary>
		/// <param name="vessel">The vessel to check.</param>
		/// <returns>Whether the vessel meets the conditions.</returns>
		protected override bool VesselMeetsCondition(Vessel vessel)
		{
			//Log("Checking VesselMeetsCondition: ", vessel.id.ToString());
			return ParameterDelegate<Vessel>.CheckChildConditions(this, vessel);
		}

		private CelestialBody cachedBody = null;
		private bool CheckDistances(Vessel vessel)
		{
			if (cachedBody == null) {
				Log("Looking for ", locationName);
				cachedBody = FlightGlobals.Bodies.Find(body => body.name == locationName);
			}
			if (cachedBody == null) {
				Log("Colud not find ", locationName);
			}
			var distance = Vector3.Distance(vessel.GetFwdVector(), cachedBody.position);
			//Log("Distance to ", locationName, " is ", distance.ToString());
			return distance >= minDistance && distance <= maxDistance;
		}

		private const string logPreix = "[PrincipiaContracts] - ";
		public static void Log(params string[] message)
		{
			var builder = StringBuilderCache.Acquire();
			builder.Append(logPreix);
			foreach (string part in message) {
				builder.Append(part);
			}
			Debug.Log(builder.ToStringAndRelease());
		}
	}
}
