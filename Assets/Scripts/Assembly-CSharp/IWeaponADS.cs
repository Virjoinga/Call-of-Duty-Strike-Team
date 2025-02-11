public interface IWeaponADS
{
	void SwitchToHips();

	void SwitchToSights();

	ADSState GetADSState();

	float GetHipsToSightsBlendAmount();
}
