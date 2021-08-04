using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutingTransactionCrossFade : MonoBehaviour, IRoutingTransaction {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Transaction(GameObject source, GameObject destination) {
        CrossFadeUI oCrossFadeUI = source.gameObject.GetComponent<CrossFadeUI>();
        oCrossFadeUI.fadeIn = source;
        oCrossFadeUI.fadeOut = destination;
        oCrossFadeUI.Fade(false);
    }
}
