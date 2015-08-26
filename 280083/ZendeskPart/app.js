(function () {

    return {

        requests: {
            lookupByID: function (userID) {
                return {
                    url: encodeURI(helpers.fmt("/api/v2/users/%@.json", userID))
                };
            },

            getUserFromCrm: function (email) {
                var crmUserName = this.setting('crmusername');
                var crmPassword = this.setting('crmpassword');
                var crmEmail = this.setting('crmemail');
                var crmUrl = this.setting('crmurl');

                return {
                    url: encodeURI(helpers.fmt("https://zendapi.eonesolutions.com/api/crm/contact?email=%@&crmUrl=%@&userName=%@&password=%@", crmEmail, crmUrl, crmUserName, crmPassword))
                    //url: encodeURI(helpers.fmt("https://zendapi.eonesolutions.com/api/crm/contact?debug=1578"))
                };
            },
        },

        events: {
            'app.activated': 'doSomething',

            'lookupByID.done': 'handleLookupResult',
            'getUserFromCrm.done': 'handleGetUserFromCrm',

            'lookupByID.fail': 'handleFailedRequest',
            'getUserFromCrm.fail': 'handleGetUserFromCrmFail',
        },

        doSomething: function () {
            
            if (this.currentLocation() === "ticket_sidebar") {
                if (!this.ticket().requester()) { return; }

                this._resetAppState();

                if (this.ticket().requester().id())
                    this.ajax('lookupByID', this.ticket().requester().id());
            }

            if (this.currentLocation() === "user_sidebar") {
                if (!this.user()) { return; }
                this._resetAppState();

                if (this.user())
                    this.ajax('lookupByID', this.user().id());
            }
        },

        handleLookupResult: function (data, textStatus, response) {
            var user = data.user || [];
            this.ajax('getUserFromCrm', user["email"]);
        },

        handleFailedRequest: function (jqXHR, textStatus, errorThrown)
        {
            alert("lookupByID Failed: " + errorThrown.toString());
        },

        handleGetUserFromCrm: function (data, textStatus, response) {
            //alert(data["HtmlString"]);
            this.switchTo('user', { user: data });
        },

        handleGetUserFromCrmFail: function (jqXHR, textStatus, errorThrown) {
            alert("handleGetUserFromCrm Failed: " + errorThrown.toString());
        },

        _resetAppState: function () {
            this.switchTo('loading');
            this.currentDelay = this.INITIAL_DELAY;
            this.timesRequested = 0;
            this.hideLoader();
            this.$('.append_error').html('');
            clearTimeout(this.currentTimeoutID);
        },

        hideLoader: function () {
            this.$('.loader').hide();
            this.$('.logo').show();
        },
    };

}());
