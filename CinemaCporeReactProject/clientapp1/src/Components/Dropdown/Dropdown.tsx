import React, { Component } from 'react';
import './Dropdown.scss';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faArrowUp, faArrowDown, faCheck } from '@fortawesome/free-solid-svg-icons'

export interface ISelectable {
    title: string,
    id: string,
}

class Type {
    title: String = '';
    list: Array<ISelectable> = [];
    selected?: ISelectable;
    resetThenSet?: (item: ISelectable) => void;
    style?: React.CSSProperties = {};
}

class State {
    listOpen: Boolean = false;
    headerTitle: String = '';
    selected?: ISelectable;
}

export default class Dropdown extends Component<Type, State>{
    constructor(props) {
        super(props);
        this.state = new State();
        const selectedElement = this.props.selected || this.props.list[0];
        this.setState({
            listOpen: false,
            headerTitle: (selectedElement && selectedElement.title) || this.props.title,
            selected: selectedElement
        });
    }



    componentDidUpdate() {
        const { listOpen } = this.state;
        if (!this.state.selected) {
            const selectedElement = this.props.selected || this.props.list[0];
            this.setState({
                headerTitle: (selectedElement && selectedElement.title) || this.props.title,
                selected: selectedElement
            })
        }
        setTimeout(() => {
            if (listOpen) {
                window.addEventListener('click', this.close);
            }
            else {
                window.removeEventListener('click', this.close);
            }
        }, 0);
    }

    componentWillUnmount() {
        window.removeEventListener('click', this.close);
    }

    close = () => {
        this.setState({
            listOpen: false
        });
    }

    selectItem = (item: ISelectable) => {
        this.setState({
            headerTitle: item.title,
            listOpen: false,
            selected: item,
        }, () => this.props.resetThenSet && this.props.resetThenSet(item));
    }

    toggleList = () => {
        this.setState(prevState => ({
            listOpen: !prevState.listOpen
        }));
    }

    render() {
        const { list } = this.props;
        const { listOpen, headerTitle } = this.state;

        return (
            <div className="wrapper" style={this.props.style}>
                <div className="header" onClick={this.toggleList}>
                    <div>{headerTitle}</div>
                    {
                        listOpen ?
                            <FontAwesomeIcon icon={faArrowUp} size="2x" />
                            : <FontAwesomeIcon icon={faArrowDown} size="2x" />
                    }
                </div>
                {listOpen &&
                    <ul className="list-container" onClick={e => e.stopPropagation()}>
                        {
                            list.map((item) => {
                                const isSelected = this.state.selected && item.id === this.state.selected.id;
                                return (
                                    <li className="item" key={item.id} onClick={() => this.selectItem(item)}>
                                        {item.title} {isSelected && <FontAwesomeIcon icon={faCheck} />}
                                    </li>
                                );
                            })
                        }
                    </ul>}
            </div>
        )
    }
}