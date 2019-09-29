import React, { Component, ChangeEvent } from 'react';
import { Cinema } from '../../Types/Cinema';
import "./CinemaComboBox.scss";
import Dropdown, { ISelectable } from '../Dropdown/Dropdown';

class Type {
    Cinemas: Array<Cinema> = [];
    style?: React.CSSProperties = {};
    itemSelected: (c: Cinema) => void = () => { };
};

export default class CinemaComboBox extends Component<Type> {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Dropdown style={this.props.style}
                title="Title"
                list={this.props.Cinemas.map(x => ({
                    title: x.name,
                    id: x.id,
                }))}
                resetThenSet={this.resetThenSet}
            />
            // <select style={this.props.style} onChange={this.onChangeHandle}>
            //     <option selected value="0">Все кинотеатры, Минск</option>
            //     {
            //         this.props.Cinemas.map(x => {
            //             return (
            //                 <option value={x.id}>{x.name}{x.address}</option>
            //             )
            //         })
            //     }
            // </select>
        )
    }

    private resetThenSet = (item: ISelectable) => {
        const i = this.props.Cinemas.filter(x => x.id === item.id)[0];
        this.props.itemSelected(i);

    }

    private onChangeHandle = (e: ChangeEvent<HTMLSelectElement>) => {
        const item = this.props.Cinemas.filter(x => x.id === e.target.value)[0];
        this.props.itemSelected(item);
    }
}