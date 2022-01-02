using Sandbox;

namespace charleroi
{
	public interface IAttackable {
		void OnAttack( Entity user, Vector3 hitpos, Vector3 normal );
	}
}
