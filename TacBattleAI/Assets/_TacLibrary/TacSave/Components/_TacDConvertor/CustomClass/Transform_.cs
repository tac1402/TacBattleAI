// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2021 Sergej Jakovlev

using UnityEngine;

namespace Tac.DConvert
{
    public class Transform_ : ICustomConvert<Transform_, Transform>
    {
        public Vector3_ position;
        public Vector3_ rotation;
        public Vector3_ scale;

        private Transform transform;

        public Transform_() { }

        public Transform_(Transform argTransform, Vector3_ argPosition, Vector3_ argRotation, Vector3_ argScale)
        {
            transform = argTransform;
            position = argPosition; rotation = argRotation; scale = argScale;
        }

        public Transform_(Transform t)
        {
            transform = t;
            ConvertFrom(t);
        }

        public void ConvertFrom(Transform t)
        {
            transform = t;
            position = new Vector3_(t.position);
            rotation = new Vector3_(t.localEulerAngles);
            scale = new Vector3_(t.localScale);
        }

        public Transform ConvertTo()
        {
            transform.position = position.ConvertTo();
            transform.localEulerAngles = rotation.ConvertTo();
            transform.localScale = scale.ConvertTo();
            return transform;
        }
    }
}