import React, { Component, ChangeEvent } from 'react';
import "./DateComboBox.scss";

class Type {
    Dates: Array<Date> = [];
    style?: React.CSSProperties = {};
    itemSelected: (d: Date) => void = () => { };
}

export default class DateComboBox extends Component<Type>{
    constructor(props) {
        super(props);
    }

    render() {
        return (

            <select style={this.props.style} onChange={this.onChangeHandle}>
                {
                    this.props.Dates.map((x, index) => {
                        return (
                            <option value={index} selected={index == 0 ? true : false}>{this.convertDateToString(x)}</option>
                        )
                    })
                }
            </select>
        );
    }

    private convertDateToString(d: Date): string {
        const o = { weekday: 'long', day: 'numeric', month: 'long' } as Intl.DateTimeFormatOptions;
        const o1 = { month: 'long', day: 'numeric' } as Intl.DateTimeFormatOptions;
        const now = new Date();
        const dayDiff = (d.getDate() - now.getDate());
        if (now.getDate() == d.getDate()) {
            return "Сегодня, " + d.toLocaleDateString(undefined, o1);
        }
        if (dayDiff >= 1 && dayDiff < 2) {
            return "Завтра, " + d.toLocaleDateString(undefined, o1);
        }
        return d.toLocaleDateString(undefined, o);
    }

    private onChangeHandle = (e: ChangeEvent<HTMLSelectElement>) => {
        const item = this.props.Dates.filter((x, index) => index.toString() === e.target.value)[0];
        this.props.itemSelected(item);
    }
}