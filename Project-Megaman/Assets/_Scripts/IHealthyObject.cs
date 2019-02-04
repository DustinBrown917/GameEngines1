

namespace MEGA
{
    public interface IHealthyObject
    {
        void RestoreToMaxHP();
        void Heal(float amount);
        void Damage(float amount);
        void Kill(bool withAnim = false);
    }
}

