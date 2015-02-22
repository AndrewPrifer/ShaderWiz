using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ShaderWiz {
    internal class CustomShader : Subshader {
        private List<Pass> _passes;

        private void Init() {
            _passes = new List<Pass>();
        }

        private void OnEnable() {
            if (_passes == null) {
                Init();
            }
        }

        public Pass GetPass(int index) {
            return _passes[index];
        }

        public IList GetPasses() {
            return _passes;
        }

        public void InsertPass(int index, PassType passType) {
            switch (passType) {
                case PassType.VertFrag:
                    _passes.Insert(index, CreateInstance<VertFragPass>());
                    break;
                case PassType.FixedFunction:
                    throw new InvalidEnumArgumentException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("passType");
            }
        }

        public void AddPass(PassType passType) {
            InsertPass(_passes.Count, passType);
        }

        public void MovePass(int from, int to) {
            _passes.Insert(to, _passes[from]);
            _passes.RemoveAt(from);
        }

        public void RemovePass(int index) {
            _passes.RemoveAt(index);
        }

        public override SubshaderType SubshaderType {
            get { return SubshaderType.Custom; }
        }
    }
}