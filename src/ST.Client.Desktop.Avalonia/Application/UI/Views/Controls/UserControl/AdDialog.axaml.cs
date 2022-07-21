using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using DynamicData.Binding;
using FluentAvalonia.Core;
using ReactiveUI;
using System.Application.Models;
using System.Application.Services;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace System.Application.UI.Views.Controls
{
    public partial class AdDialog : UserControl
    {
        readonly Button? closeButton;
        readonly CarouselBanner? banner;

        /// <summary>
        /// Defines the <see cref="AutoScroll"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> SupportCloseProperty =
            AvaloniaProperty.Register<AdDialog, bool>(nameof(SupportClose), false);

        /// <summary>
        /// Defines the <see cref="AutoScrollInterval"/> property.
        /// </summary>
        public static readonly StyledProperty<EAdvertisementStandard> StandardProperty =
            AvaloniaProperty.Register<AdDialog, EAdvertisementStandard>(nameof(Standard), EAdvertisementStandard.Horizontal);

        /// <summary>
        /// SupportClose
        /// </summary>
        public bool SupportClose
        {
            get => GetValue(SupportCloseProperty);
            set => SetValue(SupportCloseProperty, value);
        }

        /// <summary>
        /// Standard
        /// </summary>
        public EAdvertisementStandard Standard
        {
            get => GetValue(StandardProperty);
            set => SetValue(StandardProperty, value);
        }

        public AdDialog()
        {
            InitializeComponent();

            closeButton = this.FindControl<Button>("CloseAdBtn");

            if (closeButton != null)
            {
                this.GetObservable(SupportCloseProperty)
                    .Subscribe(x => closeButton.IsVisible = x);

                closeButton.Click += (s, e) =>
                {
                    RemoveAd();
                };
            }

            banner = this.FindControl<CarouselBanner>("AdBanner");
            if (banner != null)
            {
                this.GetObservable(StandardProperty)
                    .Subscribe(x =>
                    {
                        var bind = new Binding()
                        {
                            Source = AdvertiseService.Current,
                            Mode = BindingMode.OneWay,
                        };

                        if (Standard == EAdvertisementStandard.Vertical)
                        {
                            bind.Path = nameof(AdvertiseService.Current.VerticalBannerAdvertisements);
                        }
                        else
                        {
                            bind.Path = nameof(AdvertiseService.Current.HorizontalBannerAdvertisements);
                        }
                        banner.Bind(CarouselBanner.ItemsProperty, bind);
                        banner.GetObservable(CarouselBanner.ItemsProperty)
                            .Subscribe(CheckItems);
                    });
            }

            //UserService.Current.WhenValueChanged(x => x.User, false)
            //      .Subscribe(_ => Check());
            //AdvertiseService.Current.WhenValueChanged(x => x.IsShowAdvertise, false)
            //      .Subscribe(_ => Check());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        //protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        //{
        //    base.OnApplyTemplate(e);
        //}

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            if (banner != null)
                CheckItems(banner.Items);
        }

        //void Check()
        //{
        //    if (userservice.current.user != null && userservice.current.user.usertype == usertype.sponsor)
        //    {
        //        if (!advertiseservice.current.isshowad)
        //            removead();
        //    }
        //    if (advertiseservice.current.isinitialized && !advertiseservice.current.advertisements.any_nullable())
        //    {
        //        removead();
        //    }
        //}

        void CheckItems(IEnumerable x)
        {
            if (AdvertiseService.Current.IsInitialized && x.Count() == 0)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    this.IsVisible = false;
                });
            }
        }

        void RemoveAd()
        {
            Dispatcher.UIThread.Post(() =>
            {
                this.IsVisible = false;
            });
        }
    }
}