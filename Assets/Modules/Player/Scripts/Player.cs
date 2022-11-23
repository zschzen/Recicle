using Modules.Character;
using UnityEngine;

namespace Modules.Player
{
    public class Player : ABaseCharacter
    {
        public override void Move(Vector3 direction)
        {
            transform.position += direction * Time.deltaTime;
        }

        public override void Rotate(Vector3 direction)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        protected override void OnDeath()
        {
        }
    }
}