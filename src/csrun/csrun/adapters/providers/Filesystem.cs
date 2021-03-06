﻿using System;
using System.IO;
using csrun.data.domain;

namespace csrun.adapters.providers
{
    internal class Filesystem
    {
        private readonly string _templateFilename;
        public Filesystem(string templateFilename) { _templateFilename = templateFilename; }
        
        public Sourcecode ReadSource(string filename) {
            return new Sourcecode {
                Filename = filename,
                Text = File.ReadAllLines(filename)
            };
        }

        
        public string[] LoadTemplate() {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var templateFilepath = Path.Combine(exePath, _templateFilename);

            return File.ReadAllLines(templateFilepath);
        }


        public bool FileHasChanged(string filename, ref DateTime previousTimestamp) {
            var currentTimestamp = File.GetLastWriteTime(filename);
            if (currentTimestamp <= previousTimestamp) return false;
                
            previousTimestamp = currentTimestamp;
            return true;
        }
    }
}