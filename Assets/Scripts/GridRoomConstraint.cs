using System;

[Serializable]
public class GridRoomConstraint {
    public GridDoorset requiredDoors;
    public GridDoorset optionalDoors;
    public bool phantom;

    /// <summary>
    /// Can this room cell be placed on a cell with the specified doorset
    /// </summary>
    public bool CanPlace(GridDoorset on) {
        byte constraintResult = (byte)(~((requiredDoors) | (on)) | ~(~requiredDoors | ~on) | optionalDoors);
        return constraintResult == byte.MaxValue;
    }
}
