using System;

[Serializable]
public class GridRoomConstraint {
    public DoorFlags requiredDoors = new DoorFlags(QuadDirection.NONE);
    public DoorFlags optionalDoors = new DoorFlags(QuadDirection.NONE);
    public bool phantom;

    public bool CanPlace(DoorFlags on) {
        byte constraintResult = (byte)(~((requiredDoors) | (on)) | ~(~requiredDoors | ~on) | optionalDoors);
        return constraintResult == byte.MaxValue;
    }
}
