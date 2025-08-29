import React, { useState, useEffect } from 'react';
import { Dropdown } from 'primereact/dropdown';

const SafeDropdown = ({ value, options, onChange, placeholder }) => {
    const [mounted, setMounted] = useState(false);

    useEffect(() => {
        setMounted(true); // client-side render sonrası
    }, []);

    if (!mounted) return null;

    return (
        <Dropdown
            value={value}
            options={options}
            onChange={onChange}
            optionLabel="label"
            placeholder={placeholder || "Seçiniz"}
            appendTo={document.body}  // artık güvenli
            className="w-full md:w-14rem"
            autoZIndex={true}
            baseZIndex={1000}
        />
    );
};

export default SafeDropdown;
