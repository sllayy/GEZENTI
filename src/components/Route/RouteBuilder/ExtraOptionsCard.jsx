// src/components/Route/RouteBuilder/ExtraOptionsCard.jsx

import React from 'react';
import { Card } from 'primereact/card';
import { Checkbox } from 'primereact/checkbox';
import { Dropdown } from 'primereact/dropdown';

const ExtraOptionsCard = ({
    considerTraffic,
    setConsiderTraffic,
    shortestRoute,
    setShortestRoute,
    // accessibleRoute ve setAccessibleRoute kaldırıldı

    crowdPreference,
    setCrowdPreference
}) => {

    // Kalabalık seçenekleri güncellendi
    const crowdOptions = [
        { label: '0 - Çok Sakin (Doğa Yürüyüşleri)', value: 0, icon: 'pi pi-cloud' }, // Açıklamaları zenginleştirdim
        { label: '1 - Sakin (Parklar, Müzeler)', value: 1, icon: 'pi pi-sun' },
        { label: '2 - Normal (Şehir Merkezleri, Alışveriş)', value: 2, icon: 'pi pi-users' },
        { label: '3 - Hareketli (Festivaller, Canlı Mekanlar)', value: 3, icon: 'pi pi-chart-line' }
    ];


    return (
        <Card className="shadow-md">
            <div className="p-6">
                <div className="flex items-center mb-4">
                    <i className="pi pi-cog text-blue-500 mr-2"></i>
                    <h2 className="text-xl font-semibold text-gray-900">
                        Ek Seçenekler
                    </h2>
                </div>

                <div className="space-y-3">
                    <div className="flex items-center justify-between p-3 border rounded-lg">
                        <div className="flex items-center">
                            <i className="pi pi-exclamation-triangle mr-3 text-orange-500"></i>
                            <span className="font-medium">Trafik durumunu dikkate al</span>
                        </div>
                        <Checkbox
                            checked={considerTraffic}
                            onChange={(e) => setConsiderTraffic(e.checked)}
                        />
                    </div>

                    <div className="flex items-center justify-between p-3 border rounded-lg">
                        <div className="flex items-center">
                            <i className="pi pi-map-marker mr-3 text-green-500"></i>
                            <span className="font-medium">En kısa rotayı öncelikle</span>
                        </div>
                        <Checkbox
                            checked={shortestRoute}
                            onChange={(e) => setShortestRoute(e.checked)}
                        />
                    </div>

                    {/* Kalabalık Tercihi Dropdown'ı (sayısal değerler) */}
                    <div className="flex items-center justify-between p-3 border rounded-lg">
                        <div className="flex items-center">
                            <i className="pi pi-user-plus mr-3 text-purple-500"></i>
                            <span className="font-medium">Kalabalık tercihi</span>
                        </div>
                        <Dropdown
                            value={crowdPreference}
                            options={crowdOptions}
                            onChange={(e) => setCrowdPreference(e.value)}
                            optionLabel="label"
                            placeholder="Tercih Seçin"
                            className="w-full md:w-14rem"
                            appendTo="self"
                            itemTemplate={(option) => (
                                <div className="flex align-items-center">
                                    <i className={`${option.icon} mr-2`} />
                                    <div>{option.label}</div>
                                </div>
                            )}
                        />
                    </div>

                </div>
            </div>
        </Card>
    );
};

export default ExtraOptionsCard;