mergeInto(LibraryManager.library, {
    
    InitMicrophoneJS: function() {
      
        window.micContext = new (window.AudioContext || window.webkitAudioContext)();
        window.micAnalyser = window.micContext.createAnalyser();
        window.micAnalyser.fftSize = 2048;
        window.micDataArray = new Float32Array(window.micAnalyser.fftSize);

        navigator.mediaDevices.getUserMedia({ audio: true, video: false })
        .then(function(stream) {
            window.micStream = stream;
            window.micSource = window.micContext.createMediaStreamSource(stream);
            window.micSource.connect(window.micAnalyser);
            console.log("Microphone access granted!");
        })
        .catch(function(err) {
            console.error("Microphone access denied: ", err);
        });
    },

    GetMicrophoneDataJS: function(ptr, length) {
        if (!window.micAnalyser) return 0;
        
        window.micAnalyser.getFloatTimeDomainData(window.micDataArray);
        
        for (var i = 0; i < length && i < window.micDataArray.length; i++) {
            HEAPF32[(ptr >> 2) + i] = window.micDataArray[i];
        }
        return 1;
    }
});