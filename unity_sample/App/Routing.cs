using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Routing : Singleton<Routing>
{
    public List<RoutingPage> routingPgaes;

    [Serializable]
    private class RoutingPageNavigation {
        public GameObject source;
        public RoutingPage destination;
    }

    private List<RoutingPageNavigation> navigation;

    public Routing() {
        this.navigation = new List<RoutingPageNavigation>();
	}

    public RoutingPage CurrentPage {
        get { return navigation.Count == 0 ? null : this.navigation[this.navigation.Count-1].destination; }
	}

    public void NavigateForward(GameObject source, RoutingPage destination) {
        this.navigation.Add(new RoutingPageNavigation() {
            source = source,
            destination = destination
        });
        IRoutingTransaction routingTransaction = source.GetComponent<IRoutingTransaction>();
        routingTransaction.Transaction(source, destination.page);
    }

    public void NavigateForward(GameObject source, string routeName) {
        RoutingPage destination = this.routingPgaes.Find(p => p.name.ToUpper() == routeName.ToUpper());
        this.NavigateForward(source, destination);
	}

    public void NavigateBack() {
        RoutingPageNavigation current = this.navigation[this.navigation.Count - 1];
        this.navigation.RemoveAt(this.navigation.Count - 1);
        IRoutingTransaction routingTransaction = current.destination.page.GetComponent<IRoutingTransaction>();
        routingTransaction.Transaction(current.destination.page, current.source);
    }

    public RoutingPage GetPage(string routeName) {
        return this.routingPgaes.Find(p => p.name.ToUpper() == routeName.ToUpper());
    }
}
