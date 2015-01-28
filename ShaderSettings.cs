using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace ShaderWizard {
    internal class ShaderSettings : ScriptableObject {
        #region  Fields

        private List<ShaderProperty> _properties;
        private List<Subshader> _subshaders;

        #endregion

        public string Name { get; set; }
        public bool UseFallback { get; set; }
        public string Fallback { get; set; }

        public bool CommentShader { get; set; }

        private void Init() {
            _properties = new List<ShaderProperty>();
            _subshaders = new List<Subshader>();

            CommentShader = true;
            Fallback = "Diffuse";
            UseFallback = true;
            Name = "NewShader";
        }

        private void OnEnable() {
            hideFlags = HideFlags.HideAndDontSave;
            if (Name == null) {
                Init();
            }
        }

        public ShaderProperty GetProperty(int index) {
            return _properties[index];
        }

        public IList GetProperties() {
            return _properties;
        }

        public void InsertProperty(int index, PropertyType propertyType) {
            switch (propertyType) {
                case PropertyType.Range:
                    _properties.Insert(index, CreateInstance<RangeProperty>());
                    break;
                case PropertyType.Color:
                    _properties.Insert(index, CreateInstance<ColorProperty>());
                    break;
                case PropertyType.Texture:
                    _properties.Insert(index, CreateInstance<TextureProperty>());
                    break;
                case PropertyType.Float:
                    _properties.Insert(index, CreateInstance<FloatProperty>());
                    break;
                case PropertyType.Vector:
                    _properties.Insert(index, CreateInstance<VectorProperty>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException("propertyType");
            }
        }

        public void AddProperty(PropertyType propertyType) {
            InsertProperty(_properties.Count, propertyType);
        }

        public void MoveProperty(int from, int to) {
            _properties.Insert(to, _properties[from]);
            _properties.RemoveAt(from);
        }

        public void RemoveProperty(int index) {
            _properties.RemoveAt(index);
        }

        public Subshader GetSubshader(int index) {
            return _subshaders[index];
        }

        public IList GetSubshaders() {
            return _subshaders;
        }

        public void InsertSubshader(int index, SubshaderType subshaderType) {
            switch (subshaderType) {
                case SubshaderType.Surface:
                    _subshaders.Insert(index, CreateInstance<SurfaceShader>());
                    break;
                case SubshaderType.Custom:
                    throw new InvalidEnumArgumentException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("subshaderType");
            }
        }

        public void AddSubshader(SubshaderType subshaderType) {
            InsertSubshader(_subshaders.Count, subshaderType);
        }

        public void MoveSubshader(int from, int to) {
            _subshaders.Insert(to, _subshaders[from]);
            _subshaders.RemoveAt(from);
        }

        public void RemoveSubshader(int index) {
            _subshaders.RemoveAt(index);
        }
    }
}