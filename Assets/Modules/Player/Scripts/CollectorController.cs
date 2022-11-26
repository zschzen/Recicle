using System;
using DG.Tweening;
using Enums;
using Modules.Character;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Modules.Player
{
    [RequireComponent(typeof(CharacterController), typeof(Rigidbody))]
    public class CollectorController : ABaseCharacter<SOCharacterData>
    {
        [SerializeField] private CharacterController m_characterController;

        private RaycastHit[] m_hits = new RaycastHit[1];
        private Collectable.Collectable m_collectable;

        public override void Attack()
        {
        }

        public override void Move(Vector2 direction)
        {
            // Only continues if direction is not zero
            if (direction.Equals(default)) return;
            if (DOTween.IsTweening(transform)) return;
            if (!enabled) return;

            // Move the body to the direction
            var moveDirection = new Vector3(direction.x, 0, direction.y);
            m_characterController.Move(moveDirection * SpeedPerFrame);

            // smooth rotation
            var rotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15F);
        }

        public override void Rotate(Vector2 direction)
        {
        }

        protected override void OnDeath()
        {
            _ = DOTween.Kill(transform);
            _ = transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => gameObject.SetActive(false));
        }

        // Unity Methods -----------------------------------------------------------------------------------------------------------------------------------------

        // Private Methods ----------------------------------------------------------------------------------------------
        /// <summary>
        /// Tries to collect an item, if there is one in range
        /// TODO: Player must carry the collected item to the correct place
        /// </summary>
        /// <param name="callback"></param>
        internal void OnCollect(InputAction.CallbackContext callback)
        {
            // Only continues if its dead or a simple tap
            if (IsDead) return;

            // Tries to collect an item or dispose
            if (!m_collectable) TryAttachItem();
            else TryDisposeToContainer();
        }

        /// <summary>
        /// Tries to find and attach an item to the collector body.
        /// </summary>
        private void TryAttachItem()
        {
            if (!TryCubeCast(LayerMask.GetMask("Collectable"), ref m_hits, out var hit)) return;

            // If the hit has an item component
            if (!hit.collider.TryGetComponent(out Collectable.Collectable item)) return;

            m_collectable = item;

            // attach the item to the transform
            item.transform.SetParent(transform, true);

            // animate the item to go to the back of collector
            _ = item.transform.DOMove(transform.position + transform.forward * -0.5f, 0.5f);
        }

        /// <summary>
        /// Tries to find a container to dispose the item.
        /// </summary>
        private void TryDisposeToContainer()
        {
            if (!TryCubeCast(LayerMask.GetMask("Container"), ref m_hits, out var hit)) return;

            // If the hit has an Container component
            // check if container type accepts the item
            if (!hit.collider.TryGetComponent(out Container container)) return;
            if (!container.Type.HasFlag(m_collectable.Type)) return;

            // animate 
            var seq = DOTween.Sequence();
            seq.Append(m_collectable.transform.DOMove(container.transform.position, 0.5f));
            seq.Join(m_collectable.transform.DOScale(Vector3.zero, 0.5f));
            seq.AppendCallback(() =>
            {
                // Dispose the item to the container
                container.Dispose(m_collectable);

                m_collectable = default;
            });
        }
    }
}