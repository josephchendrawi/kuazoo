function createSample() {
    if (localStorage.getItem("kuazoo-sample") == null) {
        var Sample = {};
        Sample.items = [];
        localStorage.setItem("kuazoo-sample", JSON.stringify(Sample));
    }
}
function _addToSample(values) {
    localStorage.removeItem('kuazoo-sample');
    createSample();
    var SampleValue = localStorage.getItem("kuazoo-sample");
    var SampleObject = JSON.parse(SampleValue);
    var SampleCopy = SampleObject;
    var items = SampleCopy.items;
    items.push(values);
    localStorage.setItem("kuazoo-sample", JSON.stringify(SampleCopy));
}
function getSample() {
    var SampleValue = localStorage.getItem("kuazoo-sample");
    var SampleObject = JSON.parse(SampleValue);
    return SampleObject;
}