using System;
using ContractConfigurator.Parameters;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace PrincipiaContracts
{
	class LagrangeParameter : VesselParameter
	{

		public static string CHILD_BODY_NAME = "childBodyName";
		public static string CHILD_DISTANCE = "childDistance";
		public static string CHILD_TOLERANCE = "childTolerance";
		public static string PARENT_BODY_NAME = "parentBodyName";
		public static string PARENT_DISTANCE = "parentDistance";
		public static string PARENT_TOLERANCE = "parentTolerance";
		public static string ANGLE = "angle";
		public static string ANGLE_TOLERANCE = "angleTolerance";

		private string childBodyName;
		private string parentBodyName;
		private double childDistance;
		private double childTolerance;
		private double parentDistance;
		private double parentTolerance;

		private double angle;
		private double angleTolerance;

		private double minChildDistance;
		private double maxChildDistance;

		private double minParentDistance;
		private double maxParentDistance;

		private double minAngle;
		private double maxAngle;

		public LagrangeParameter() : base(null)
		{

		}
		public LagrangeParameter(string title) : base(title)
		{

		}
		public LagrangeParameter(string childBodyName, string parentBodyName, double childDistance, double childTolerance, double parentDistance, double parentTolerance, double angle, double angleTolerance, string title = null) : base(title)
		{
			this.childBodyName = childBodyName;
			this.parentBodyName = parentBodyName;

			this.childDistance = childDistance;
			this.childTolerance = childTolerance;

			this.parentDistance = parentDistance;
			this.parentTolerance = parentTolerance;

			this.angle = angle;
			this.angleTolerance = angleTolerance;

			SetMinMaxValues();

			CreateDelegates();
		}


		protected override string GetParameterTitle()
		{
			if (!string.IsNullOrEmpty(title)) {
				return title;
			}

			return "Reach the proper " + parentBodyName + "-" + childBodyName + " Lagrange point";
		}

		protected override string GetNotes()
		{
			if (!string.IsNullOrEmpty(notes)) {
				return notes;
			}
			StringBuilder sb = StringBuilderCache.Acquire();
			sb.Append(BuildMinDistanceNote(parentBodyName, minParentDistance));
			sb.Append(Environment.NewLine);
			sb.Append(BuildMaxDistanceNote(parentBodyName, maxParentDistance));
			sb.Append(Environment.NewLine);

			sb.Append(BuildMinDistanceNote(childBodyName, minChildDistance));
			sb.Append(Environment.NewLine);
			sb.Append(BuildMaxDistanceNote(childBodyName, maxChildDistance));

			if (angleTolerance > 0.0) {
				sb.Append(Environment.NewLine);
				sb.Append(BuildMinAngleNote(childBodyName, minAngle));
				sb.Append(Environment.NewLine);
				sb.Append(BuildMaxAngleNote(childBodyName, maxAngle));
			}
			return sb.ToStringAndRelease();
		}

		private string BuildMinDistanceNote(string bodyName, double distance)
		{
			return BuildDistanceNote(bodyName, "at least", distance);
		}

		private string BuildMaxDistanceNote(string bodyName, double distance)
		{
			return BuildDistanceNote(bodyName, "at most", distance);
		}

		private string BuildDistanceNote(string bodyName, string qualifier, double distance)
		{
			return "Be " + qualifier + " " + Utils.FormatDistance(distance) + " from " + bodyName;
		}

		private string BuildMinAngleNote(string bodyName, double angle)
		{
			return BuildAngleNote(bodyName, "at least", angle);
		}

		private string BuildMaxAngleNote(string bodyName, double angle)
		{
			return BuildAngleNote(bodyName, "at most", angle);
		}
		private string BuildAngleNote(string bodyName, string qualifier, double angle)
		{
			return "Have an angle " + qualifier + " " + Utils.FormatAngle(angle) + " off of " + bodyName + "'s prograte orbital vector";
		}

		protected override void OnParameterSave(ConfigNode node)
		{
			base.OnParameterSave(node);
			node.AddValue(CHILD_BODY_NAME, childBodyName);
			node.AddValue(CHILD_DISTANCE, childDistance);
			node.AddValue(CHILD_TOLERANCE, childTolerance);

			node.AddValue(PARENT_BODY_NAME, parentBodyName);
			node.AddValue(PARENT_DISTANCE, parentDistance);
			node.AddValue(PARENT_TOLERANCE, parentTolerance);

			if (angleTolerance > 0.0) {
				node.AddValue(ANGLE, angle);
				node.AddValue(ANGLE_TOLERANCE, angleTolerance);
			}
		}

		protected override void OnParameterLoad(ConfigNode node)
		{
			try {
				base.OnParameterLoad(node);
				childBodyName = node.GetValue(CHILD_BODY_NAME);
				childDistance = Convert.ToDouble(node.GetValue(CHILD_DISTANCE));
				childTolerance = Convert.ToDouble(node.GetValue(CHILD_TOLERANCE));

				parentBodyName = node.GetValue(PARENT_BODY_NAME);
				parentDistance = Convert.ToDouble(node.GetValue(PARENT_DISTANCE));
				parentTolerance = Convert.ToDouble(node.GetValue(PARENT_TOLERANCE));

				if (node.HasValue(ANGLE_TOLERANCE)) {
					angle = Convert.ToDouble(node.GetValue(ANGLE));
					angleTolerance = Convert.ToDouble(node.GetValue(ANGLE_TOLERANCE));
				}

				SetMinMaxValues();

				CreateDelegates();
			}
			finally {
				ParameterDelegate<Vessel>.OnDelegateContainerLoad(node);
			}
		}

		private void SetMinMaxValues()
		{
			minChildDistance = childDistance - childTolerance;
			maxChildDistance = childDistance + childTolerance;
			minParentDistance = parentDistance - parentTolerance;
			maxParentDistance = parentDistance + parentTolerance;
			minAngle = angle - angleTolerance;
			maxAngle = angle + angleTolerance;
		}

		protected void CreateDelegates()
		{
			AddParameter(new ParameterDelegate<Vessel>(BuildMinDistanceNote(childBodyName, minChildDistance), vessel => CheckMinDistance(vessel, childBodyName, minChildDistance)));
			AddParameter(new ParameterDelegate<Vessel>(BuildMaxDistanceNote(childBodyName, maxChildDistance), vessel => CheckMaxDistance(vessel, childBodyName, maxChildDistance)));
			AddParameter(new ParameterDelegate<Vessel>(BuildMinDistanceNote(parentBodyName, minParentDistance), vessel => CheckMinDistance(vessel, parentBodyName, minParentDistance)));
			AddParameter(new ParameterDelegate<Vessel>(BuildMaxDistanceNote(parentBodyName, maxParentDistance), vessel => CheckMaxDistance(vessel, parentBodyName, maxParentDistance)));

			if (angleTolerance > 0.0) {
				AddParameter(new ParameterDelegate<Vessel>(BuildMinAngleNote(childBodyName, minAngle), vessel => CheckMinAngle(vessel, childBodyName, minAngle)));
				AddParameter(new ParameterDelegate<Vessel>(BuildMaxAngleNote(childBodyName, maxAngle), vessel => CheckMaxAngle(vessel, childBodyName, maxAngle)));
			}
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
			return ParameterDelegate<Vessel>.CheckChildConditions(this, vessel);
		}

		private bool CheckMinDistance(Vessel vessel, string bodyName, double minDistance)
		{
			double distance = GetDistanceFromBody(vessel, bodyName);
			return distance >= minDistance;
		}

		private bool CheckMaxDistance(Vessel vessel, string bodyName, double maxDistance)
		{
			double distance = GetDistanceFromBody(vessel, bodyName);
			return distance <= maxDistance;
		}

		private bool CheckMinAngle(Vessel vessel, string bodyName, double minAngle)
		{
			double angle = GetAngleFromBody(vessel, bodyName);
			return angle >= minAngle;
		}

		private bool CheckMaxAngle(Vessel vessel, string bodyName, double maxAngle)
		{
			double angle = GetAngleFromBody(vessel, bodyName);
			return angle <= maxAngle;
		}

		private double GetDistanceFromBody(Vessel vessel, string bodyName)
		{
			CelestialBody body = GetCelestialBody(bodyName);
			double distance = Vector3d.Distance(vessel.GetWorldPos3D(), body.position);
			Utils.Log("Distance to ", bodyName, ": ", distance.ToString());
			return distance;
		}

		private double GetAngleFromBody(Vessel vessel, string bodyName)
		{
			CelestialBody body = GetCelestialBody(bodyName);
			Vector3d bodyMinusVessel = vessel.GetWorldPos3D() - body.position;
			double angle = Vector3d.Angle(bodyMinusVessel, body.getRFrmVelOrbit(body.GetOrbit()));
			Utils.Log("Angle to ", bodyName, "'s prograde vector: ", angle.ToString());
			return angle;
		}

		private Dictionary<string, CelestialBody> cachedBodies = new Dictionary<string, CelestialBody>();
		private CelestialBody GetCelestialBody(string bodyName)
		{
			if (!cachedBodies.ContainsKey(bodyName)) {
				CelestialBody celestialBody = FlightGlobals.Bodies.Find(body => body.name == bodyName);
				if (celestialBody == null) {
					Utils.Log("ERROR: cannot find body " + bodyName);
					// Further calls on this object will produce a NRE, but honestly this should have already been validated in the LagrangeFactory
					return null;
				}

				cachedBodies.Add(bodyName, FlightGlobals.Bodies.Find(body => body.name == bodyName));
			}
			return cachedBodies[bodyName];
		}
	}
}
