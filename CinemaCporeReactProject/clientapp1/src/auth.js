"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Auth = {
    isAuthenticated: false,
    authenticate: function (callback) {
        this.isAuthenticated = true;
    },
    signout: function (callback) {
        this.isAuthenticated = false;
    }
};
exports.default = Auth;
//# sourceMappingURL=auth.js.map