
namespace MEGA
{
    public interface IEnergyObject
    {
        void RestoreToMaxEnergy();
        void GainEnergy(float amount);
        void LoseEnergy(float amount);
    }
}

