using System;
using UnityEngine;

public interface IInteractor {
    public Type InteractorType { get; }

    public GameObject GameObject => ((MonoBehaviour)this).gameObject;


    public Krampus GetPlayer() {
        if (InteractorType != Type.Player) throw new System.Exception("Invalid interactor type");
        return (Krampus)this;
    }

    [Flags]
    public enum Type {
        None = 0,
        Player = 1,
        NPC = 2,
        Other = 4
    }
}

