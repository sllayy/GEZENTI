import React from 'react';
import { Card } from 'primereact/card';
import { Checkbox } from 'primereact/checkbox';
import Select from 'react-select';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCloud, faSun, faUsers, faChartLine } from '@fortawesome/free-solid-svg-icons';


const ExtraOptionsCard = ({
    considerTraffic,
    setConsiderTraffic,
    shortestRoute,
    setShortestRoute,
    crowdPreference,
    setCrowdPreference
}) => {

    const crowdOptions = [
        { value: 0, label: '0 - Çok Sakin (Doğa Yürüyüşleri)', icon: faCloud },
        { value: 1, label: '1 - Sakin (Parklar, Müzeler)', icon: faSun },
        { value: 2, label: '2 - Normal (Şehir Merkezleri, Alışveriş)', icon: faUsers },
        { value: 3, label: '3 - Hareketli (Festivaller, Canlı Mekanlar)', icon: faChartLine }
    ];



    return (
        <Card className="shadow-md rounded-xlg">
            <div className="p-6 space-y-4 rounded-xlg">
                <div className="flex items-center mb-4">
                    <i className="pi pi-cog text-blue-500 mr-2"></i>
                    <h2 className="text-xl font-semibold text-gray-900">
                        Ek Seçenekler
                    </h2>
                </div>

                <div className="space-y-3">
                    <div className="flex items-center justify-between p-3 border rounded-xlg">
                        <div className="flex items-center">
                            <i className="pi pi-exclamation-triangle mr-3 text-orange-500"></i>
                            <span className="font-medium">Trafik durumunu dikkate al</span>
                        </div>
                        <Checkbox
                            className="ml-4 border border-gray-500 rounded-sm"
                            checked={considerTraffic}
                            onChange={(e) => setConsiderTraffic(e.checked)}
                        />
                    </div>

                    <div className="flex items-center justify-between p-3 border rounded-xlg">
                        <div className="flex items-center">
                            <i className="pi pi-map-marker mr-3 text-green-500"></i>
                            <span className="font-medium">En kısa rotayı öncelikle</span>
                        </div>
                        <Checkbox
                            className="ml-4 border border-gray-500 rounded-sm"
                            checked={shortestRoute}
                            onChange={(e) => setShortestRoute(e.checked)}
                        />
                    </div>

                    <div className="flex flex-col md:flex-row items-start md:items-center p-3 border rounded-xlg min-h-[80px] space-y-2 md:space-y-0 md:space-x-4">
                        <div className="flex items-center flex-shrink-0">
                            <i className="pi pi-user-plus mr-3 text-purple-500"></i>
                            <span className="font-medium">Kalabalık tercihi</span>
                        </div>
                        <div className="flex-1 w-full">
                            <Select
                                value={crowdOptions.find(opt => opt.value === crowdPreference)}
                                onChange={(selected) => setCrowdPreference(selected ? selected.value : null)}
                                options={crowdOptions}
                                placeholder="Tercih Seçin"
                                formatOptionLabel={option => (
                                    <div className="flex items-center">
                                        <FontAwesomeIcon className="mr-2 text-purple-500" icon={option.icon} />
                                        <span>{option.label}</span>
                                    </div>
                                )}
                                className="w-full"
                            />
                        </div>
                    </div>
                </div>
            </div>
        </Card>
    );
};

export default ExtraOptionsCard;