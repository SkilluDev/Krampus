using System;
using static QuadDirection;

public abstract class QuadDirectional<TContainer, TElement> where TContainer : QuadDirectional<TContainer, TElement>, new() {
    public abstract TElement North { get; set; }
    public abstract TElement East { get; set; }
    public abstract TElement South { get; set; }
    public abstract TElement West { get; set; }
    public TElement this[QuadDirection index] {
        get => index switch {
            NORTH => North,
            EAST => East,
            SOUTH => South,
            WEST => West,
            _ => throw new Exception("Cannot get multiple elements via indexing!")
        };
        set {
            var _ = index switch {
                NORTH => North = value,
                EAST => East = value,
                SOUTH => South = value,
                WEST => West = value,
                _ => throw new Exception("Cannot set multiple elements via indexing!")
            };
        }
    }

    public virtual TContainer Rotate90Clockwise() {
        return new TContainer() {
            North = this.West,
            East = this.North,
            South = this.East,
            West = this.South
        };
    }

    public virtual TContainer Invert() {
        return new TContainer() {
            North = this.South,
            East = this.West,
            South = this.North,
            West = this.East
        };
    }

    public virtual TContainer InvertHorizontal() {
        return new TContainer() {
            North = this.North,
            East = this.West,
            South = this.South,
            West = this.East
        };
    }

    public virtual TContainer InvertVertical() {
        return new TContainer() {
            North = this.South,
            East = this.East,
            South = this.North,
            West = this.West
        };
    }
}
