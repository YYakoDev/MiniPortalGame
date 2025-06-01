using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool AlreadyInteracted { get; set; }
    public Vector3 Offset { get; }
    public AudioClip InteractSfx { get; }
    public void Interact();
}
