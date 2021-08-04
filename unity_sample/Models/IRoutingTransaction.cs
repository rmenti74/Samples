using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoutingTransaction {

    void Transaction(GameObject source, GameObject destination);

}
