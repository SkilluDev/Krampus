using System;

[Serializable]
public class GridRoomConstraint {
    public GridDoorset requiredDoors = new GridDoorset(QuadDirection.NONE);
    public GridDoorset optionalDoors = new GridDoorset(QuadDirection.NONE);
    public bool phantom;

    public bool CanPlace(GridDoorset on) {
        byte constraintResult = (byte)(~((requiredDoors) | (on)) | ~(~requiredDoors | ~on) | optionalDoors);
        return constraintResult == byte.MaxValue;
    }
}
