//-*- coding: utf-8 -*-
// 2013-07-10

// code based on
// JavaScript Book by David Flanagan
// http://xahlee.info/comp/js_book_man_made_complexity.html

// Convert element e to relative positioning and "shake" it left and right.
// The first argument can be an element object or the id of an element.
// If a function is passed as the second argument, it will be invoked
// with e as an argument when the animation is complete.
// The 3rd argument specifies how far to shake e. The default is 5 pixels.
// The 4th argument specifies how long to shake for. The default is 500 ms.
function shake(e, oncomplete, distance, time) {
    // Handle arguments
    if (typeof e === "string") e = document.getElementById(e);
    if (!time) time = 500;
    if (!distance) distance = 5;

    // Save the original style of e, Make e relatively positioned, Note the animation start time, Start the animation
    var originalStyle = e.style.cssText;
    e.style.position = "relative";
    var start = (new Date()).getTime();
    animate();

    // This function checks the elapsed time and updates the position of e.
    // If the animation is complete, it restores e to its original state.
    // Otherwise, it updates e's position and schedules itself to run again.
    function animate() {
        var now = (new Date()).getTime();
        // Get current time
        var elapsed = now - start;
        // How long since we started
        var fraction = elapsed / time;
        // What fraction of total time?
        if (fraction < 1) {
            // If the animation is not yet complete
            // Compute the x position of e as a function of animation
            // completion fraction. We use a sinusoidal function, and multiply
            // the completion fraction by 4pi, so that it shakes back and
            // forth twice.
            var x = distance * Math.sin(fraction * 4 * Math.PI);
            e.style.left = x + "px";
            // Try to run again in 25ms or at the end of the total time.
            // We're aiming for a smooth 40 frames/second animation.
            setTimeout(animate, Math.min(25, time - elapsed));
        }
        else {
            // Otherwise, the animation is complete
            e.style.cssText = originalStyle // Restore the original style
            if (oncomplete) oncomplete(e);
            // Invoke completion callback
        }
    }
}
