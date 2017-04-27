using ContractConfigurator;
using Contracts;

namespace PrincipiaContracts
{
	public class LagrangeFactory : ParameterFactory
	{

		protected string childBodyName;
		protected double childDistance;
		protected double childTolerance;

		protected double parentDistance;
		protected double parentTolerance;

		protected double angle;
		protected double angleTolerance;

		private string parentBodyName;

		public override bool Load(ConfigNode configNode)
		{
			bool valid = base.Load(configNode);


			valid &= ConfigNodeUtil.ParseValue<string>(configNode, LagrangeParameter.CHILD_BODY_NAME, x => childBodyName = x, this);
			valid &= ConfigNodeUtil.ParseValue(configNode, LagrangeParameter.CHILD_DISTANCE, x => childDistance = x, this, 0.0, x => Validation.GE(x, 0.0));
			valid &= ConfigNodeUtil.ParseValue(configNode, LagrangeParameter.CHILD_TOLERANCE, x => childTolerance = x, this, double.MaxValue, x => Validation.GE(x, 0.0));

			valid &= ConfigNodeUtil.ParseValue(configNode, LagrangeParameter.PARENT_DISTANCE, x => parentDistance = x, this, 0.0, x => Validation.GE(x, 0.0));
			valid &= ConfigNodeUtil.ParseValue(configNode, LagrangeParameter.PARENT_TOLERANCE, x => parentTolerance = x, this, double.MaxValue, x => Validation.GE(x, 0.0));

			valid &= ConfigNodeUtil.ParseValue(configNode, LagrangeParameter.ANGLE, x => angle = x, this, 0.0, x => Validation.BetweenInclusive(x, 0.0, 180.0));
			valid &= ConfigNodeUtil.ParseValue(configNode, LagrangeParameter.ANGLE_TOLERANCE, x => angleTolerance = x, this, 0.0, x => Validation.BetweenInclusive(x, 0.0, 180.0));

			valid &= ValidateBodies(childBodyName);

			return valid;
		}
		public override ContractParameter Generate(Contract contract)
		{
			return new LagrangeParameter(childBodyName, parentBodyName, childDistance, childTolerance, parentDistance, parentTolerance, angle, angleTolerance);
		}

		private bool ValidateBodies(string childBodyName)
		{
			CelestialBody childBody = FlightGlobals.Bodies.Find(body => body.name == childBodyName);
			if (childBody == null) {
				Utils.Log("Could not find child body: ", childBodyName);
				return false;
			}
			CelestialBody parentBody = childBody.referenceBody;
			if (parentBody == null) {
				Utils.Log(childBodyName, " has no parent body");
				return false;
			}
			parentBodyName = parentBody.name;
			return true;
		}

	}
}
