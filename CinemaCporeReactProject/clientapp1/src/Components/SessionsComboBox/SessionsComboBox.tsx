import React, { Component } from 'react';

class type {
    style?: React.CSSProperties = {};
}

export default class SessionsComboBox extends Component<type> {
    constructor(props) {
        super(props);
    }
    render() {
        return (
            <select style={this.props.style}>
                <option value="0">Все сеансы</option>
            </select>
        )
    }
}