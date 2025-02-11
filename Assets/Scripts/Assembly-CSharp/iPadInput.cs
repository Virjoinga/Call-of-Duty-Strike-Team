public interface iPadInput
{
	bool IsConnected();

	void ToggleDebugDraw();

	void Reset();

	bool Down(int button);

	bool Pressed(int button);

	bool Released(int button);

	bool Held(int button);

	bool AxisPressedPositive(int axis);

	bool AxisPressedNegative(int axis);

	bool AxisHeldPositive(int axis);

	float AxisValue(int axis);
}
