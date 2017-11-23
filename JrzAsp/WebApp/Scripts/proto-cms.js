// requires JQuery v1.10+
// requires VueJS v2.5+
var protoCms = {};
protoCms.apiBasePath = "/proto-cms-api/v1";
protoCms.utils = {};
protoCms.mixins = {};
protoCms.vues = {};

(function(protoCms) {
    var uuu = protoCms.utils;
    var vvv = protoCms.vues;

    vvv.modals = {};
    vvv.shownModals = [];

    uuu.onBeforeCallApi = [];
    uuu.onSuccessCallApi = [];
    uuu.onErrorCallApi = [];
    uuu.onDoneCallApi = [];

    uuu.callApi = function(ajaxOptions) {
        ajaxOptions = ajaxOptions || {};
        var bHandlers = uuu.onBeforeCallApi;
        var sHandlers = uuu.onSuccessCallApi;
        var eHandlers = uuu.onErrorCallApi;
        var dHandlers = uuu.onDoneCallApi;

        for (var i = 0; i < bHandlers.length; i++) {
            bHandlers[i](ajaxOptions);
        }
        return $.ajax(ajaxOptions)
            .done(function(data, textStatus, jqXHR) {
                for (var i = 0; i < bHandlers.length; i++) {
                    sHandlers[i](data, textStatus, jqXHR);
                }
            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                for (var i = 0; i < bHandlers.length; i++) {
                    eHandlers[i](jqXHR, textStatus, errorThrown);
                }
            })
            .always(function(dataOrJqXHR, textStatus, jqXHROrErrorThrown) {
                for (var i = 0; i < bHandlers.length; i++) {
                    dHandlers[i](dataOrJqXHR, textStatus, jqXHROrErrorThrown);
                }
            });
    };

    uuu.getdef = function(val, defaultValue) {
        if (val === null || val === undefined) return defaultValue;
        return val;
    };

    uuu.toSimpleJson = function(obj) {
        return JSON.parse(JSON.stringify(obj));
    };

    uuu.addModal = function(varName, modalVueOptions) {
        modalVueOptions.el = null;
        var mdl = new Vue(modalVueOptions);
        uuu.addCreatedModal(varName, mdl, true);
        return mdl;
    };

    uuu.createModalContainer = function(varName) {
        var randId = "modal-widget-" + Math.round(Math.random() * 100000000);
        $("#cms-modal-widget-container")
            .append('<div id="' + randId + '" data-cms-modal-widget="' + varName + '"></div>');
        var mountEl = "div#" + randId;
        return mountEl;
    }

    uuu.addCreatedModal = function(varName, modalVue, mountModalVue) {
        if (uuu.hasModal(varName)) {
            console.error("ProtoCMS: there's already a modal with var name '" + varName + "'.");
        }
        vvv.modals[varName] = modalVue;
        if (mountModalVue) {
            $(function () {
                var mountEl = uuu.createModalContainer(varName);
                modalVue.$mount(mountEl);
            });
        }
        return modalVue;
    }

    uuu.hasModal = function(varName) {
        return (_.has(vvv.modals, varName));
    };

    uuu.getModal = function(varName) {
        if (uuu.hasModal(varName)) {
            return vvv.modals[varName];
        }
        return null;
    };

    uuu.showModal = function(varName) {
        _.forIn(vvv.modals,
            function(val, key) {
                if (key === varName) {
                    var waitForHide = null;
                    var checkAllHidden = function() {
                        if (vvv.shownModals.length === 0) {
                            if (waitForHide !== null) {
                                window.clearInterval(waitForHide);
                                waitForHide = null;
                            }
                            val.$children[0].showModal();
                        }
                    };
                    waitForHide = window.setInterval(checkAllHidden, 1);
                    checkAllHidden();
                } else {
                    val.$children[0].hideModal();
                }
            });
    };

    uuu.hideModal = function() {
        _.forIn(vvv.modals,
            function(val, key) {
                val.$children[0].hideModal();
            });
    };

    uuu.removeModal = function(varName) {
        var mdl = null;
        _.forIn(vvv.modals,
            function(val, key) {
                if (key === varName) {
                    mdl = val;
                }
                val.$children[0].hideModal();
            });
        if (mdl !== null) {
            vvv.modals[varName] = null;
            var waitForHide = null;
            var checkAllHidden = function() {
                if (vvv.shownModals.length === 0) {
                    if (waitForHide !== null) {
                        window.clearInterval(waitForHide);
                        waitForHide = null;
                    }
                    mdl.$destroy();
                    for (var i = 0; i < mdl.$children.length; i++) {
                        $(mdl.$children[i].$el).remove();
                    }
                }
            };
            waitForHide = window.setInterval(checkAllHidden, 1);
            checkAllHidden();
        }
    };

    uuu.popupEntityOperationForm = function (entityApiName, entityId, entityTypeId, operationName, title, subtitle) {
        var modalVarName = "entityOperationForm";
        var existingModal = vvv[modalVarName];
        if (!existingModal) {
            existingModal = new Vue({
                template: [
                    "<cms-modal-page-entity-operation ref='modOpModal' ",
                    ":entity-api-name='entityApiName' ",
                    ":entity-id='entityId' ",
                    ":entity-type-id='entityTypeId' ",
                    ":operation-name='operationName' ",
                    ":title='title' ",
                    ":subtitle='subtitle'",
                    ">",
                    "</cms-modal-page-entity-operation>"
                ].join(""),
                data: {
                    entityApiName: entityApiName,
                    entityId: entityId,
                    entityTypeId: entityTypeId,
                    operationName: operationName,
                    title: title,
                    subtitle: subtitle
                },
                methods: {
                    init: function (entityApiName, entityId, entityTypeId, operationName, title, subtitle) {
                        this.$refs.modOpModal.init(entityApiName, entityId, entityTypeId, operationName, title, subtitle);
                    }
                }
            });
            $(function() {
                var mountEl = uuu.createModalContainer(modalVarName);
                existingModal.$mount(mountEl);
            });
            vvv.modals[modalVarName] = existingModal.$children[0];

        } else {
            existingModal.init(entityApiName, entityId, entityTypeId, operationName, title, subtitle);
        }
        uuu.showModal(modalVarName);
        return existingModal;
    };

    uuu.popupEntityRawDataViewer = function (entityApiName, entityId, entityTypeId, title, subtitle) {
        var modalVarName = "entityRawDataViewer";
        var existingModal = vvv[modalVarName];
        if (!existingModal) {
            existingModal = new Vue({
                template: [
                    "<cms-modal-page-entity-raw-viewer ref='modOpModal' ",
                    ":entity-api-name='entityApiName' ",
                    ":entity-id='entityId' ",
                    ":entity-type-id='entityTypeId' ",
                    ":title='title' ",
                    ":subtitle='subtitle' ",
                    ">",
                    "</cms-modal-page-entity-raw-viewer>"
                ].join(""),
                data: {
                    entityApiName: entityApiName,
                    entityId: entityId,
                    entityTypeId: entityTypeId,
                    title: title,
                    subtitle: subtitle
                },
                methods: {
                    init: function (entityApiName, entityId, entityTypeId, title, subtitle) {
                        this.$refs.modOpModal.init(entityApiName, entityId, entityTypeId, title, subtitle);
                    }
                }
            });
            $(function () {
                var mountEl = uuu.createModalContainer(modalVarName);
                existingModal.$mount(mountEl);
            });
            vvv.modals[modalVarName] = existingModal.$children[0];

        } else {
            existingModal.init(entityApiName, entityId, entityTypeId, title, subtitle);
        }
        uuu.showModal(modalVarName);
        return existingModal;
    };
})(protoCms);