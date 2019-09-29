import React, { Component } from 'react';

class Type {
    style?: React.CSSProperties = {};
}

export default class FormatComboBox extends Component<Type> {
    constructor(props) {
        super(props);
    }
    render() {
        return (
            <select style={this.props.style}>
                <option>
                    <input type="checkbox" />>
                    <label >Текст</label>
                </option>
            </select>
        )
    }
}